using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using YANEDGE.EmailManagement.Application.BackgroundJobs;
using YANEDGE.EmailManagement.EntityFrameworkCore;
using YANEDGE.EmailManagement.Middleware;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace YANEDGE.EmailManagement;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule),
    typeof(EmailManagementApplicationModule),
    typeof(EmailManagementEntityFrameworkCoreModule),
    typeof(EmailManagementHttpApiModule)
)]
public class EmailManagementHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        ConfigureCors(context, configuration);
        ConfigureSwaggerServices(context, configuration);
        ConfigureHangfire(context, configuration);
        ConfigureHealthChecks(context, configuration);
    }

    private void ConfigureHangfire(ServiceConfigurationContext context, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        context.Services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UsePostgreSqlStorage(options =>
                  {
                      options.UseNpgsqlConnection(connectionString);
                  });
        });

        context.Services.AddHangfireServer(options =>
        {
            options.WorkerCount = 5;
            options.ServerName = "EmailManagementServer";
        });
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim().TrimEnd('/'))
                            .ToArray() ?? Array.Empty<string>()
                    )
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    private void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "邮件管理系统 API",
                Version = "v1",
                Description = "YANEDGE Email Management System RESTful API",
                Contact = new OpenApiContact
                {
                    Name = "YANEDGE Team",
                    Email = "support@yanedge.com"
                }
            });

            options.DocInclusionPredicate((docName, description) => true);
            options.CustomSchemaIds(type => type.FullName);
        });
    }

    private void ConfigureHealthChecks(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("Application is running"))
            .AddCheck("database", () =>
            {
                try
                {
                    using var scope = context.Services.BuildServiceProvider().CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<EmailManagementDbContext>();
                    var canConnect = dbContext.Database.CanConnect();
                    return canConnect
                        ? HealthCheckResult.Healthy("Database connection successful")
                        : HealthCheckResult.Unhealthy("Cannot connect to database");
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy("Database health check failed", ex);
                }
            },
            tags: new[] { "db", "postgresql" });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        // 全局异常处理（必须在最前面）
        if (!env.IsDevelopment())
        {
            app.UseGlobalExceptionHandler();
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();
        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAbpSerilogEnrichers();

        // 健康检查端点
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        timestamp = DateTime.UtcNow,
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            duration = e.Value.Duration.TotalMilliseconds,
                            tags = e.Value.Tags
                        })
                    });
                    await context.Response.WriteAsync(result);
                }
            });

            // 快速活性检查
            endpoints.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = _ => false
            });

            // 就绪检查
            endpoints.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("db") || check.Tags.Contains("hangfire")
            });
        });

        // Hangfire Dashboard
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
        });

        app.UseConfiguredEndpoints();

        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "邮件管理系统 API v1");
            options.DocumentTitle = "邮件管理系统 - API文档";
        });

        // 注册Hangfire后台任务
        ConfigureBackgroundJobs();
    }

    private void ConfigureBackgroundJobs()
    {
        // 邮件同步任务 - 每5分钟执行一次
        RecurringJob.AddOrUpdate<MailSyncJob>(
            "mail-sync",
            job => job.ExecuteAsync(),
            "*/5 * * * *"); // Cron: 每5分钟

        // 发件任务处理 - 每1分钟执行一次
        RecurringJob.AddOrUpdate<SendTaskProcessorJob>(
            "send-task-processor",
            job => job.ExecuteAsync(),
            "* * * * *"); // Cron: 每1分钟

        // 规则执行任务 - 每10分钟执行一次
        RecurringJob.AddOrUpdate<RuleExecutionJob>(
            "rule-execution",
            job => job.ExecuteAsync(),
            "*/10 * * * *"); // Cron: 每10分钟

        // 失败任务重试 - 每30分钟执行一次
        RecurringJob.AddOrUpdate<FailedTaskRetryJob>(
            "failed-task-retry",
            job => job.ExecuteAsync(),
            "*/30 * * * *"); // Cron: 每30分钟
    }
}

/// <summary>
/// Hangfire Dashboard授权过滤器
/// 生产环境需要管理员权限才能访问
/// </summary>
public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // 开发环境允许所有访问
        if (httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            return true;
        }

        // 生产环境需要认证且具有管理员角色
        var user = httpContext.User;
        return user.Identity?.IsAuthenticated == true &&
               (user.IsInRole("admin") || user.IsInRole("Admin"));
    }
}
