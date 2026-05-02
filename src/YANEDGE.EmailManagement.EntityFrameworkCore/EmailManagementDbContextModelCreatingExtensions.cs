using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using YANEDGE.EmailManagement.Domain.MailAccount;
using YANEDGE.EmailManagement.Domain.MailMessage;
using YANEDGE.EmailManagement.Domain.MailThread;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Domain.Approval;
using YANEDGE.EmailManagement.Domain.Collaboration;

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
    }
}
