using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Application.Contracts.MailAccount;

/// <summary>
/// 邮箱账号DTO
/// </summary>
public class MailAccountDto
{
    public Guid Id { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public MailAccountType AccountType { get; set; }
    public string AccountTypeName { get; set; } = string.Empty;
    public bool SyncEnabled { get; set; }
    public bool SendEnabled { get; set; }
    public int HealthStatus { get; set; }
    public string HealthStatusName { get; set; } = string.Empty;
    public DateTime? LastSyncAt { get; set; }
    public DateTime? LastSendAt { get; set; }
}
