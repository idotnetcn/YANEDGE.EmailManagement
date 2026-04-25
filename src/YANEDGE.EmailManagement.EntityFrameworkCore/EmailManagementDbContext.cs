using Microsoft.EntityFrameworkCore;
using YANEDGE.EmailManagement.Accounts;
using YANEDGE.EmailManagement.Auditing;
using YANEDGE.EmailManagement.BusinessRelations;
using YANEDGE.EmailManagement.Conversations;
using YANEDGE.EmailManagement.Integrations;
using YANEDGE.EmailManagement.Rules;
using YANEDGE.EmailManagement.Statistics;
using YANEDGE.EmailManagement.Tags;
using YANEDGE.EmailManagement.Templates;
using YANEDGE.EmailManagement.Transport;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace YANEDGE.EmailManagement.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class EmailManagementDbContext : AbpDbContext<EmailManagementDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    // Identity
    public DbSet<Volo.Abp.Identity.IdentityUser> Users { get; set; } = null!;
    public DbSet<Volo.Abp.Identity.IdentityRole> Roles { get; set; } = null!;
    public DbSet<Volo.Abp.Identity.IdentityClaimType> ClaimTypes { get; set; } = null!;
    public DbSet<Volo.Abp.Identity.OrganizationUnit> OrganizationUnits { get; set; } = null!;
    public DbSet<Volo.Abp.Identity.IdentitySecurityLog> SecurityLogs { get; set; } = null!;
    public DbSet<Volo.Abp.Identity.IdentityLinkUser> LinkUsers { get; set; } = null!;
    public DbSet<Volo.Abp.Identity.IdentityUserDelegation> UserDelegations { get; set; } = null!;
    public DbSet<Volo.Abp.Identity.IdentitySession> Sessions { get; set; } = null!;

    // TenantManagement
    public DbSet<Volo.Abp.TenantManagement.Tenant> Tenants { get; set; } = null!;
    public DbSet<Volo.Abp.TenantManagement.TenantConnectionString> TenantConnectionStrings { get; set; } = null!;

    // MailAccounts
    public DbSet<MailAccount> MailAccounts { get; set; } = null!;
    public DbSet<MailAccountCredential> MailAccountCredentials { get; set; } = null!;
    public DbSet<MailAccountSyncPolicy> MailAccountSyncPolicies { get; set; } = null!;
    public DbSet<MailAccountUserPermission> MailAccountUserPermissions { get; set; } = null!;
    public DbSet<MailAccountRolePermission> MailAccountRolePermissions { get; set; } = null!;
    public DbSet<MailAccountOuPermission> MailAccountOuPermissions { get; set; } = null!;
    public DbSet<MailAccountHealthLog> MailAccountHealthLogs { get; set; } = null!;
    public DbSet<MailAccountAccessLog> MailAccountAccessLogs { get; set; } = null!;

    // Conversations
    public DbSet<MailThread> MailThreads { get; set; } = null!;
    public DbSet<MailMessage> MailMessages { get; set; } = null!;
    public DbSet<MailMessageBody> MailMessageBodies { get; set; } = null!;
    public DbSet<MailRecipient> MailRecipients { get; set; } = null!;
    public DbSet<MailMessageHeader> MailMessageHeaders { get; set; } = null!;
    public DbSet<MailAttachment> MailAttachments { get; set; } = null!;
    public DbSet<MailFolder> MailFolders { get; set; } = null!;
    public DbSet<MailFolderMapping> MailFolderMappings { get; set; } = null!;
    public DbSet<MailUserMessageState> MailUserMessageStates { get; set; } = null!;
    public DbSet<MailThreadParticipant> MailThreadParticipants { get; set; } = null!;
    public DbSet<MailAssignment> MailAssignments { get; set; } = null!;
    public DbSet<MailInternalComment> MailInternalComments { get; set; } = null!;
    public DbSet<MailProcessingRecord> MailProcessingRecords { get; set; } = null!;
    public DbSet<MailTodo> MailTodos { get; set; } = null!;
    public DbSet<MailApproval> MailApprovals { get; set; } = null!;

    // Tags
    public DbSet<MailTag> MailTags { get; set; } = null!;
    public DbSet<MailMessageTag> MailMessageTags { get; set; } = null!;

    // Templates
    public DbSet<MailTemplateCategory> MailTemplateCategories { get; set; } = null!;
    public DbSet<MailTemplate> MailTemplates { get; set; } = null!;
    public DbSet<MailTemplateVersion> MailTemplateVersions { get; set; } = null!;
    public DbSet<MailTemplateVariable> MailTemplateVariables { get; set; } = null!;
    public DbSet<MailSignature> MailSignatures { get; set; } = null!;

    // Rules
    public DbSet<MailRule> MailRules { get; set; } = null!;
    public DbSet<MailRuleCondition> MailRuleConditions { get; set; } = null!;
    public DbSet<MailRuleAction> MailRuleActions { get; set; } = null!;
    public DbSet<MailRuleExecutionLog> MailRuleExecutionLogs { get; set; } = null!;
    public DbSet<MailDomainPolicy> MailDomainPolicies { get; set; } = null!;

    // Business Relations
    public DbSet<Contact> Contacts { get; set; } = null!;
    public DbSet<BusinessObjectType> BusinessObjectTypes { get; set; } = null!;
    public DbSet<BusinessObjectRelation> BusinessObjectRelations { get; set; } = null!;

    // Transport
    public DbSet<MailSendTask> MailSendTasks { get; set; } = null!;
    public DbSet<MailSendTaskLog> MailSendTaskLogs { get; set; } = null!;
    public DbSet<MailSyncTask> MailSyncTasks { get; set; } = null!;
    public DbSet<MailSyncTaskLog> MailSyncTaskLogs { get; set; } = null!;

    // Integrations
    public DbSet<IntegrationApp> IntegrationApps { get; set; } = null!;
    public DbSet<IntegrationWebhook> IntegrationWebhooks { get; set; } = null!;
    public DbSet<IntegrationEventLog> IntegrationEventLogs { get; set; } = null!;
    public DbSet<ExternalMapping> ExternalMappings { get; set; } = null!;
    public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; } = null!;

    // Auditing
    public DbSet<MailAuditLog> MailAuditLogs { get; set; } = null!;
    public DbSet<ConfigurationChangeLog> ConfigurationChangeLogs { get; set; } = null!;

    // Statistics
    public DbSet<MailStatisticsDaily> MailStatisticsDaily { get; set; } = null!;
    public DbSet<MailProcessingStatisticsDaily> MailProcessingStatisticsDaily { get; set; } = null!;

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
