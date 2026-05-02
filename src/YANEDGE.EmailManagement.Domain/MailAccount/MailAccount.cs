using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.MailAccount;

/// <summary>
/// 邮箱账号聚合根
/// </summary>
public class MailAccount : AggregateRoot<Guid>
{
    /// <summary>
    /// 账号名称
    /// </summary>
    public string AccountName { get; private set; }

    /// <summary>
    /// 邮箱地址
    /// </summary>
    public string EmailAddress { get; private set; }

    /// <summary>
    /// 显示名
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// 账号类型
    /// </summary>
    public MailAccountType AccountType { get; private set; }

    /// <summary>
    /// 收件协议
    /// </summary>
    public MailProtocol IncomingProtocol { get; private set; }

    /// <summary>
    /// 收件服务器地址
    /// </summary>
    public string IncomingHost { get; private set; }

    /// <summary>
    /// 收件服务器端口
    /// </summary>
    public int IncomingPort { get; private set; }

    /// <summary>
    /// 收件是否使用SSL
    /// </summary>
    public bool IncomingSslEnabled { get; private set; }

    /// <summary>
    /// 发件协议
    /// </summary>
    public MailProtocol OutgoingProtocol { get; private set; }

    /// <summary>
    /// 发件服务器地址
    /// </summary>
    public string OutgoingHost { get; private set; }

    /// <summary>
    /// 发件服务器端口
    /// </summary>
    public int OutgoingPort { get; private set; }

    /// <summary>
    /// 发件是否使用SSL
    /// </summary>
    public bool OutgoingSslEnabled { get; private set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    /// 密码(加密存储)
    /// </summary>
    public string EncryptedPassword { get; private set; }

    /// <summary>
    /// 是否启用同步
    /// </summary>
    public bool SyncEnabled { get; private set; }

    /// <summary>
    /// 是否允许发件
    /// </summary>
    public bool SendEnabled { get; private set; }

    /// <summary>
    /// 最后同步时间
    /// </summary>
    public DateTime? LastSyncAt { get; private set; }

    /// <summary>
    /// 最后发件时间
    /// </summary>
    public DateTime? LastSendAt { get; private set; }

    /// <summary>
    /// 健康状态
    /// </summary>
    public int HealthStatus { get; private set; }

    /// <summary>
    /// 所属组织ID
    /// </summary>
    public Guid? OwnerOrgId { get; private set; }

    private MailAccount()
    {
        // For ORM
        AccountName = string.Empty;
        EmailAddress = string.Empty;
        DisplayName = string.Empty;
        IncomingHost = string.Empty;
        OutgoingHost = string.Empty;
        Username = string.Empty;
        EncryptedPassword = string.Empty;
    }

    public MailAccount(
        Guid id,
        string accountName,
        string emailAddress,
        string displayName,
        MailAccountType accountType,
        MailProtocol incomingProtocol,
        string incomingHost,
        int incomingPort,
        bool incomingSslEnabled,
        MailProtocol outgoingProtocol,
        string outgoingHost,
        int outgoingPort,
        bool outgoingSslEnabled,
        string username,
        string encryptedPassword,
        Guid? ownerOrgId = null) : base(id)
    {
        AccountName = accountName;
        EmailAddress = emailAddress;
        DisplayName = displayName;
        AccountType = accountType;
        IncomingProtocol = incomingProtocol;
        IncomingHost = incomingHost;
        IncomingPort = incomingPort;
        IncomingSslEnabled = incomingSslEnabled;
        OutgoingProtocol = outgoingProtocol;
        OutgoingHost = outgoingHost;
        OutgoingPort = outgoingPort;
        OutgoingSslEnabled = outgoingSslEnabled;
        Username = username;
        EncryptedPassword = encryptedPassword;
        OwnerOrgId = ownerOrgId;
        SyncEnabled = true;
        SendEnabled = true;
        HealthStatus = 1; // Healthy
    }

    public void EnableSync()
    {
        SyncEnabled = true;
    }

    public void DisableSync()
    {
        SyncEnabled = false;
    }

    public void EnableSend()
    {
        SendEnabled = true;
    }

    public void DisableSend()
    {
        SendEnabled = false;
    }

    public void UpdateLastSyncTime(DateTime syncTime)
    {
        LastSyncAt = syncTime;
    }

    public void UpdateLastSendTime(DateTime sendTime)
    {
        LastSendAt = sendTime;
    }

    public void UpdateHealthStatus(int status)
    {
        HealthStatus = status;
    }
}
