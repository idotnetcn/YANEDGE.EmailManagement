using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace YANEDGE.EmailManagement.EntityFrameworkCore;

public static class EmailManagementDbContextModelCreatingExtensions
{
    public static void ConfigureEmailManagement(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
        // Configuration for EmailManagement domain entities will be added here
    }
}

