using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace YANEDGE.EmailManagement.Domain.Attachment;

/// <summary>
/// 附件访问日志实体
/// </summary>
public class AttachmentAccessLog : CreationAuditedEntity<Guid>
{
    /// <summary>
    /// 附件ID
    /// </summary>
    public Guid AttachmentId { get; private set; }

    /// <summary>
    /// 访问用户ID
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// 访问类型 (View/Download)
    /// </summary>
    public string AccessType { get; private set; }

    /// <summary>
    /// 访问IP地址
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// 用户代理
    /// </summary>
    public string? UserAgent { get; private set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccessful { get; private set; }

    /// <summary>
    /// 失败原因
    /// </summary>
    public string? FailureReason { get; private set; }

    protected AttachmentAccessLog()
    {
        AccessType = string.Empty;
    }

    public AttachmentAccessLog(
        Guid id,
        Guid attachmentId,
        Guid userId,
        string accessType,
        bool isSuccessful,
        string? ipAddress = null,
        string? userAgent = null,
        string? failureReason = null
    ) : base(id)
    {
        AttachmentId = attachmentId;
        UserId = userId;
        AccessType = accessType;
        IsSuccessful = isSuccessful;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        FailureReason = failureReason;
    }
}
