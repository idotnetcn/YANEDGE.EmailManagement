using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using YANEDGE.EmailManagement.Emails;

namespace YANEDGE.EmailManagement.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class EmailManagementDbContext : AbpDbContext<EmailManagementDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    public DbSet<Email> Emails { get; set; }
    public DbSet<EmailRecipient> EmailRecipients { get; set; }
    public DbSet<EmailAttachment> EmailAttachments { get; set; }
    public DbSet<EmailTemplate> EmailTemplates { get; set; }

    // Identity
    public DbSet<Volo.Abp.Identity.IdentityUser> Users { get; set; }
    public DbSet<Volo.Abp.Identity.IdentityRole> Roles { get; set; }
    public DbSet<Volo.Abp.Identity.IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<Volo.Abp.Identity.OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<Volo.Abp.Identity.IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<Volo.Abp.Identity.IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<Volo.Abp.Identity.IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<Volo.Abp.Identity.IdentitySession> Sessions { get; set; }

    // TenantManagement
    public DbSet<Volo.Abp.TenantManagement.Tenant> Tenants { get; set; }
    public DbSet<Volo.Abp.TenantManagement.TenantConnectionString> TenantConnectionStrings { get; set; }

    public EmailManagementDbContext(DbContextOptions<EmailManagementDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureTenantManagement();
        builder.ConfigureEmailManagement();
    }
}
