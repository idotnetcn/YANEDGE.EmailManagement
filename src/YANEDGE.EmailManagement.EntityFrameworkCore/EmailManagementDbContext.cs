using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using YANEDGE.EmailManagement.Domain.MailAccount;
using YANEDGE.EmailManagement.Domain.MailMessage;
using YANEDGE.EmailManagement.Domain.MailThread;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Domain.Approval;
using YANEDGE.EmailManagement.Domain.Collaboration;
using YANEDGE.EmailManagement.Template;
using YANEDGE.EmailManagement.Label;
using YANEDGE.EmailManagement.Rule;
using YANEDGE.EmailManagement.Attachment;
using YANEDGE.EmailManagement.Contact;
using YANEDGE.EmailManagement.BusinessRelation;

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

    // Template Management
    public DbSet<MailTemplate> MailTemplates { get; set; } = null!;
    public DbSet<MailSignature> MailSignatures { get; set; } = null!;

    // Label Management
    public DbSet<MailLabel> MailLabels { get; set; } = null!;
    public DbSet<MailMessageLabel> MailMessageLabels { get; set; } = null!;

    // Rule Management
    public DbSet<MailRule> MailRules { get; set; } = null!;
    public DbSet<RuleExecutionLog> RuleExecutionLogs { get; set; } = null!;

    // Attachment Management
    public DbSet<MailAttachment> MailAttachments { get; set; } = null!;
    public DbSet<AttachmentAccessLog> AttachmentAccessLogs { get; set; } = null!;

    // Contact Management
    public DbSet<MailContact> MailContacts { get; set; } = null!;

    // Business Relations
    public DbSet<MailBusinessRelation> MailBusinessRelations { get; set; } = null!;

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
