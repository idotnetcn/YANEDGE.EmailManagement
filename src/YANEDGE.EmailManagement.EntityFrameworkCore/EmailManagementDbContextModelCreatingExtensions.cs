using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using YANEDGE.EmailManagement.Emails;

namespace YANEDGE.EmailManagement.EntityFrameworkCore;

public static class EmailManagementDbContextModelCreatingExtensions
{
    public static void ConfigureEmailManagement(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<Email>(b =>
        {
            b.ToTable("Emails");
            b.HasKey(x => x.Id);
            b.Property(x => x.Subject).IsRequired().HasMaxLength(EmailConsts.MaxSubjectLength);
            b.Property(x => x.Body).IsRequired();
            b.Property(x => x.FromAddress).IsRequired().HasMaxLength(EmailConsts.MaxAddressLength);
            b.Property(x => x.FromDisplayName).HasMaxLength(EmailConsts.MaxAddressLength);
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.Priority).IsRequired();
            b.HasMany(x => x.Recipients).WithOne().HasForeignKey(x => x.EmailId).IsRequired();
            b.HasMany(x => x.Attachments).WithOne().HasForeignKey(x => x.EmailId).IsRequired();
        });

        builder.Entity<EmailRecipient>(b =>
        {
            b.ToTable("EmailRecipients");
            b.HasKey(x => x.Id);
            b.Property(x => x.Address).IsRequired().HasMaxLength(EmailConsts.MaxAddressLength);
            b.Property(x => x.DisplayName).HasMaxLength(EmailConsts.MaxAddressLength);
            b.Property(x => x.RecipientType).IsRequired();
        });

        builder.Entity<EmailAttachment>(b =>
        {
            b.ToTable("EmailAttachments");
            b.HasKey(x => x.Id);
            b.Property(x => x.FileName).IsRequired().HasMaxLength(256);
            b.Property(x => x.ContentType).IsRequired().HasMaxLength(128);
            b.Property(x => x.StoragePath).IsRequired().HasMaxLength(1024);
        });

        builder.Entity<EmailTemplate>(b =>
        {
            b.ToTable("EmailTemplates");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(EmailConsts.MaxTemplateNameLength);
            b.Property(x => x.Subject).IsRequired().HasMaxLength(EmailConsts.MaxSubjectLength);
            b.Property(x => x.Body).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();
        });
    }
}
