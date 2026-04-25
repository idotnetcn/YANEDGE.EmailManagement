using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using YANEDGE.EmailManagement.EntityFrameworkCore;

namespace YANEDGE.EmailManagement.DbMigrator;

public class EmailManagementDbMigrationService : ITransientDependency
{
    private readonly ILogger<EmailManagementDbMigrationService> _logger;
    private readonly EmailManagementDbContext _dbContext;

    public EmailManagementDbMigrationService(
        ILogger<EmailManagementDbMigrationService> logger,
        EmailManagementDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task MigrateAsync()
    {
        _logger.LogInformation("Starting database migration...");
        await _dbContext.Database.MigrateAsync();
        _logger.LogInformation("Database migration completed.");
    }
}
