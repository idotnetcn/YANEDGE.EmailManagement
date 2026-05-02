using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using YANEDGE.EmailManagement.Domain.MailAccount;
using YANEDGE.EmailManagement.Domain.MailMessage;
using YANEDGE.EmailManagement.Domain.MailThread;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Domain.Approval;
using YANEDGE.EmailManagement.Domain.Collaboration;
using YANEDGE.EmailManagement.Domain.Template;
using YANEDGE.EmailManagement.Domain.Label;
using YANEDGE.EmailManagement.Domain.Rule;
using YANEDGE.EmailManagement.Domain.Attachment;
using YANEDGE.EmailManagement.Domain.Contact;
using YANEDGE.EmailManagement.Domain.BusinessRelation;

namespace YANEDGE.EmailManagement.EntityFrameworkCore;

public static class EmailManagementDbContextModelCreatingExtensions
{
    public static void ConfigureEmailManagement(this ModelBuilder builder)
    {
        builder.Entity<MailAccount>(b =>
        {
            b.ToTable("MailAccounts");
            b.ConfigureByConvention();

            b.Property(x => x.AccountName).IsRequired().HasMaxLength(200);
            b.Property(x => x.EmailAddress).IsRequired().HasMaxLength(256);
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(200);
            b.Property(x => x.IncomingHost).IsRequired().HasMaxLength(200);
            b.Property(x => x.OutgoingHost).IsRequired().HasMaxLength(200);
            b.Property(x => x.Username).IsRequired().HasMaxLength(256);
            b.Property(x => x.EncryptedPassword).IsRequired().HasMaxLength(500);

            b.HasIndex(x => x.EmailAddress);
        });

        builder.Entity<MailThread>(b =>
        {
            b.ToTable("MailThreads");
            b.ConfigureByConvention();

            b.Property(x => x.Subject).IsRequired().HasMaxLength(500);
            b.Property(x => x.NormalizedSubject).IsRequired().HasMaxLength(500);

            b.HasIndex(x => x.MailAccountId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => new { x.CurrentAssigneeType, x.CurrentAssigneeId });
            b.HasIndex(x => x.LatestMessageTime);
        });

        builder.Entity<MailMessage>(b =>
        {
            b.ToTable("MailMessages");
            b.ConfigureByConvention();

            b.Property(x => x.Subject).IsRequired().HasMaxLength(500);
            b.Property(x => x.FromAddress).IsRequired().HasMaxLength(256);
            b.Property(x => x.FromDisplayName).HasMaxLength(200);
            b.Property(x => x.InternetMessageId).HasMaxLength(500);

            b.HasIndex(x => x.MailAccountId);
            b.HasIndex(x => x.ThreadId);
            b.HasIndex(x => x.InternetMessageId);
            b.HasIndex(x => x.ReceivedTime);
        });

        builder.Entity<MailSendTask>(b =>
        {
            b.ToTable("MailSendTasks");
            b.ConfigureByConvention();

            b.Property(x => x.Subject).IsRequired().HasMaxLength(500);
            b.Property(x => x.IdempotencyKey).HasMaxLength(100);
            b.Property(x => x.ExternalBizRef).HasMaxLength(200);
            b.Property(x => x.ErrorCode).HasMaxLength(100);

            b.HasIndex(x => x.MailAccountId);
            b.HasIndex(x => x.ThreadId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.CreatedByUserId);
            b.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter("[IdempotencyKey] IS NOT NULL");
            b.HasIndex(x => x.ExternalBizRef);
        });

        builder.Entity<MailApproval>(b =>
        {
            b.ToTable("MailApprovals");
            b.ConfigureByConvention();

            b.Property(x => x.BusinessType).IsRequired().HasMaxLength(100);

            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.CurrentApproverId);
            b.HasIndex(x => new { x.BusinessType, x.BusinessId });
        });

        builder.Entity<ThreadAssignment>(b =>
        {
            b.ToTable("ThreadAssignments");
            b.ConfigureByConvention();

            b.Property(x => x.AssignmentType).IsRequired().HasMaxLength(50);
            b.Property(x => x.Reason).HasMaxLength(500);

            b.HasIndex(x => x.ThreadId);
            b.HasIndex(x => new { x.ToAssigneeType, x.ToAssigneeId });
            b.HasIndex(x => x.AssignedAt);
        });

        builder.Entity<InternalNote>(b =>
        {
            b.ToTable("InternalNotes");
            b.ConfigureByConvention();

            b.Property(x => x.Content).IsRequired();

            b.HasIndex(x => x.ThreadId);
            b.HasIndex(x => x.CreatedByUserId);
            b.HasIndex(x => x.IsPinned);
        });

        // Template Management
        builder.Entity<MailTemplate>(b =>
        {
            b.ToTable("MailTemplates");
            b.ConfigureByConvention();

            b.Property(x => x.Code).IsRequired().HasMaxLength(100);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Category).HasMaxLength(100);
            b.Property(x => x.Language).HasMaxLength(50);
            b.Property(x => x.Description).HasMaxLength(1000);
            b.Property(x => x.SubjectTemplate).IsRequired().HasMaxLength(500);
            b.Property(x => x.BodyTemplate).IsRequired();
            b.Property(x => x.PlainTextTemplate);

            b.HasIndex(x => x.Code).IsUnique();
            b.HasIndex(x => x.Category);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.IsDefault);
        });

        builder.Entity<MailSignature>(b =>
        {
            b.ToTable("MailSignatures");
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Content).IsRequired();
            b.Property(x => x.PlainTextContent);

            b.HasIndex(x => x.Scope);
            b.HasIndex(x => x.OwnerUserId);
            b.HasIndex(x => x.OwnerOrganizationId);
            b.HasIndex(x => x.IsDefault);
        });

        // Label Management
        builder.Entity<MailLabel>(b =>
        {
            b.ToTable("MailLabels");
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
            b.Property(x => x.Color).HasMaxLength(50);
            b.Property(x => x.Description).HasMaxLength(500);

            b.HasIndex(x => x.Name);
            b.HasIndex(x => x.OwnerUserId);
            b.HasIndex(x => x.OwnerOrganizationId);
            b.HasIndex(x => x.IsSystemLabel);
        });

        builder.Entity<MailMessageLabel>(b =>
        {
            b.ToTable("MailMessageLabels");
            b.HasKey(x => new { x.MailMessageId, x.LabelId });
            b.ConfigureByConvention();

            b.HasIndex(x => x.MailMessageId);
            b.HasIndex(x => x.LabelId);
        });

        // Rule Management
        builder.Entity<MailRule>(b =>
        {
            b.ToTable("MailRules");
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Description).HasMaxLength(1000);

            // 配置复杂类型为JSON
            b.Property(x => x.ApplicableMailAccountIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null) ?? new List<Guid>())
                .HasColumnType("jsonb");

            b.Property(x => x.Conditions)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<RuleCondition>>(v, (JsonSerializerOptions)null) ?? new List<RuleCondition>())
                .HasColumnType("jsonb")
                .IsRequired();

            b.Property(x => x.Actions)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<RuleAction>>(v, (JsonSerializerOptions)null) ?? new List<RuleAction>())
                .HasColumnType("jsonb")
                .IsRequired();

            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.Priority);
        });

        builder.Entity<RuleExecutionLog>(b =>
        {
            b.ToTable("RuleExecutionLogs");
            b.ConfigureByConvention();

            b.Property(x => x.ExecutionResult).HasMaxLength(2000);
            b.Property(x => x.ErrorMessage).HasMaxLength(2000);

            b.HasIndex(x => x.RuleId);
            b.HasIndex(x => x.MailMessageId);
            b.HasIndex(x => x.ThreadId);
            b.HasIndex(x => x.CreationTime);
        });

        // Attachment Management
        builder.Entity<MailAttachment>(b =>
        {
            b.ToTable("MailAttachments");
            b.ConfigureByConvention();

            b.Property(x => x.FileName).IsRequired().HasMaxLength(500);
            b.Property(x => x.ContentType).HasMaxLength(200);
            b.Property(x => x.StoragePath).IsRequired().HasMaxLength(1000);
            b.Property(x => x.FileHash).HasMaxLength(100);
            b.Property(x => x.ContentId).HasMaxLength(200);
            b.Property(x => x.ScanResult).HasMaxLength(500);

            b.HasIndex(x => x.MailMessageId);
            b.HasIndex(x => x.FileHash);
            b.HasIndex(x => x.IsSensitive);
            b.HasIndex(x => x.IsScanned);
        });

        builder.Entity<AttachmentAccessLog>(b =>
        {
            b.ToTable("AttachmentAccessLogs");
            b.ConfigureByConvention();

            b.Property(x => x.AccessType).IsRequired().HasMaxLength(50);
            b.Property(x => x.IpAddress).HasMaxLength(100);
            b.Property(x => x.UserAgent).HasMaxLength(500);
            b.Property(x => x.FailureReason).HasMaxLength(500);

            b.HasIndex(x => x.AttachmentId);
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.CreationTime);
        });

        // Contact Management
        builder.Entity<MailContact>(b =>
        {
            b.ToTable("MailContacts");
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.EmailAddress).IsRequired().HasMaxLength(256);
            b.Property(x => x.PhoneNumber).HasMaxLength(50);
            b.Property(x => x.CompanyName).HasMaxLength(200);
            b.Property(x => x.JobTitle).HasMaxLength(100);
            b.Property(x => x.Source).HasMaxLength(50);
            b.Property(x => x.ExternalId).HasMaxLength(200);
            b.Property(x => x.Notes).HasMaxLength(1000);

            b.HasIndex(x => x.EmailAddress);
            b.HasIndex(x => x.CustomerId);
            b.HasIndex(x => x.SupplierId);
            b.HasIndex(x => x.ExternalId);
            b.HasIndex(x => x.IsActive);
        });

        // Business Relations
        builder.Entity<MailBusinessRelation>(b =>
        {
            b.ToTable("MailBusinessRelations");
            b.ConfigureByConvention();

            b.Property(x => x.BusinessObjectId).IsRequired().HasMaxLength(100);
            b.Property(x => x.BusinessObjectName).HasMaxLength(500);
            b.Property(x => x.BusinessObjectCode).HasMaxLength(200);
            b.Property(x => x.RelationSource).HasMaxLength(50);
            b.Property(x => x.ExternalSystem).HasMaxLength(100);

            b.HasIndex(x => x.MailMessageId);
            b.HasIndex(x => x.ThreadId);
            b.HasIndex(x => new { x.BusinessObjectType, x.BusinessObjectId });
            b.HasIndex(x => x.IsPrimary);
        });
    }
}
