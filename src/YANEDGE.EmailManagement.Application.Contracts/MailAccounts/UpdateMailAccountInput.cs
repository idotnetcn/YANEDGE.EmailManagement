using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.MailAccounts;

public class UpdateMailAccountInput
{
    [Required, MaxLength(128)]
    public string Name { get; set; } = null!;

    [MaxLength(128)]
    public string? DisplayName { get; set; }

    [MaxLength(256)]
    public string? ImapHost { get; set; }
    public int? ImapPort { get; set; }

    [MaxLength(256)]
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }

    public bool UseSsl { get; set; } = true;
    public int SyncIntervalSeconds { get; set; } = 300;

    [MaxLength(512)]
    public string? Description { get; set; }
}
