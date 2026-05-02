using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Application.Contracts.MailAccount;

/// <summary>
/// 创建邮箱账号输入
/// </summary>
public class CreateMailAccountInput
{
    public string AccountName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public MailAccountType AccountType { get; set; }
    public MailProtocol IncomingProtocol { get; set; }
    public string IncomingHost { get; set; } = string.Empty;
    public int IncomingPort { get; set; }
    public bool IncomingSslEnabled { get; set; }
    public MailProtocol OutgoingProtocol { get; set; }
    public string OutgoingHost { get; set; } = string.Empty;
    public int OutgoingPort { get; set; }
    public bool OutgoingSslEnabled { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool SyncEnabled { get; set; } = true;
    public bool SendEnabled { get; set; } = true;
    public Guid? OwnerOrgId { get; set; }
}
