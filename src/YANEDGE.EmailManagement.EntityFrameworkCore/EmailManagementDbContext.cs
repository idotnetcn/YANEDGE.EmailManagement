using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Domain.MailAccount;
using YANEDGE.EmailManagement.Domain.MailMessage;
using YANEDGE.EmailManagement.Domain.MailThread;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Domain.Approval;
using YANEDGE.EmailManagement.Domain.Collaboration;

namespace YANEDGE.EmailManagement.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class EmailManagementDbContext : AbpDbContext<EmailManagementDbContext>
{
    public DbSet<MailAccount> MailAccounts { get; set; } = null!;
    public DbSet<MailMessage> MailMessages { get; set; } = null!;
    public DbSet<MailThread> MailThreads { get; set; } = null!;
    public DbSet<MailSendTask> MailSendTasks { get; set; } = null!;
    public DbSet<MailApproval> MailApprovals { get; set; } = null!;
    public DbSet<ThreadAssignment> ThreadAssignments { get; set; } = null!;
    public DbSet<InternalNote> InternalNotes { get; set; } = null!;

    public EmailManagementDbContext(DbContextOptions<EmailManagementDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureEmailManagement();
    }
}
