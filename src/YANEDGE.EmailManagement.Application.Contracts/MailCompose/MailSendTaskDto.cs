using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Application.Contracts.MailCompose;

/// <summary>
/// 发件任务DTO
/// </summary>
public class MailSendTaskDto
{
    public Guid Id { get; set; }
    public Guid MailAccountId { get; set; }
    public Guid? ThreadId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public SendTaskStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public bool NeedApproval { get; set; }
    public Guid? ApprovalId { get; set; }
    public DateTime? ScheduledSendTime { get; set; }
    public DateTime? ActualSentTime { get; set; }
    public int RetryCount { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
