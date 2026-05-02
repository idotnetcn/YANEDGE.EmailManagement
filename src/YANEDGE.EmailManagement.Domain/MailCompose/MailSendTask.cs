using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.MailCompose;

/// <summary>
/// 发件任务聚合根
/// </summary>
public class MailSendTask : AggregateRoot<Guid>
{
    /// <summary>
    /// 邮箱账号ID
    /// </summary>
    public Guid MailAccountId { get; private set; }

    /// <summary>
    /// 线程ID
    /// </summary>
    public Guid? ThreadId { get; private set; }

    /// <summary>
    /// 邮件主题
    /// </summary>
    public string Subject { get; private set; }

    /// <summary>
    /// HTML正文
    /// </summary>
    public string? BodyHtml { get; private set; }

    /// <summary>
    /// 纯文本正文
    /// </summary>
    public string? BodyText { get; private set; }

    /// <summary>
    /// 模板ID
    /// </summary>
    public Guid? TemplateId { get; private set; }

    /// <summary>
    /// 签名ID
    /// </summary>
    public Guid? SignatureId { get; private set; }

    /// <summary>
    /// 发件任务状态
    /// </summary>
    public SendTaskStatus Status { get; private set; }

    /// <summary>
    /// 是否需要审批
    /// </summary>
    public bool NeedApproval { get; private set; }

    /// <summary>
    /// 关联审批ID
    /// </summary>
    public Guid? ApprovalId { get; private set; }

    /// <summary>
    /// 计划发送时间
    /// </summary>
    public DateTime? ScheduledSendTime { get; private set; }

    /// <summary>
    /// 实际发送时间
    /// </summary>
    public DateTime? ActualSentTime { get; private set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; private set; }

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetryCount { get; private set; }

    /// <summary>
    /// 错误代码
    /// </summary>
    public string? ErrorCode { get; private set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// 外部业务引用
    /// </summary>
    public string? ExternalBizRef { get; private set; }

    /// <summary>
    /// 幂等键
    /// </summary>
    public string? IdempotencyKey { get; private set; }

    /// <summary>
    /// 创建用户ID
    /// </summary>
    public Guid CreatedByUserId { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    private MailSendTask()
    {
        // For ORM
        Subject = string.Empty;
    }

    public MailSendTask(
        Guid id,
        Guid mailAccountId,
        string subject,
        Guid createdByUserId,
        bool needApproval = false) : base(id)
    {
        MailAccountId = mailAccountId;
        Subject = subject;
        CreatedByUserId = createdByUserId;
        Status = SendTaskStatus.Draft;
        NeedApproval = needApproval;
        RetryCount = 0;
        MaxRetryCount = 3;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetBody(string? htmlBody, string? textBody)
    {
        BodyHtml = htmlBody;
        BodyText = textBody;
    }

    public void SetTemplate(Guid templateId)
    {
        TemplateId = templateId;
    }

    public void SetSignature(Guid signatureId)
    {
        SignatureId = signatureId;
    }

    public void SetThread(Guid threadId)
    {
        ThreadId = threadId;
    }

    public void SetExternalBizRef(string externalBizRef)
    {
        ExternalBizRef = externalBizRef;
    }

    public void SetIdempotencyKey(string idempotencyKey)
    {
        IdempotencyKey = idempotencyKey;
    }

    public void SetScheduledSendTime(DateTime scheduledTime)
    {
        ScheduledSendTime = scheduledTime;
    }

    public void SubmitForApproval(Guid approvalId)
    {
        if (Status != SendTaskStatus.Draft)
        {
            throw new InvalidOperationException("Only draft tasks can be submitted for approval");
        }

        Status = SendTaskStatus.PendingApproval;
        ApprovalId = approvalId;
        NeedApproval = true;
    }

    public void ApprovalApproved()
    {
        if (Status != SendTaskStatus.PendingApproval)
        {
            throw new InvalidOperationException("Only tasks pending approval can be approved");
        }

        Status = SendTaskStatus.PendingSend;
    }

    public void ApprovalRejected()
    {
        if (Status != SendTaskStatus.PendingApproval)
        {
            throw new InvalidOperationException("Only tasks pending approval can be rejected");
        }

        Status = SendTaskStatus.Draft;
    }

    public void StartSending()
    {
        if (Status != SendTaskStatus.PendingSend)
        {
            throw new InvalidOperationException("Only pending send tasks can start sending");
        }

        Status = SendTaskStatus.Sending;
    }

    public void MarkAsSent(DateTime sentTime)
    {
        if (Status != SendTaskStatus.Sending)
        {
            throw new InvalidOperationException("Only sending tasks can be marked as sent");
        }

        Status = SendTaskStatus.Sent;
        ActualSentTime = sentTime;
    }

    public void MarkAsFailed(string errorCode, string errorMessage)
    {
        Status = SendTaskStatus.Failed;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public void IncrementRetryCount()
    {
        RetryCount++;
    }

    public bool CanRetry()
    {
        return Status == SendTaskStatus.Failed && RetryCount < MaxRetryCount;
    }

    public void Retry()
    {
        if (!CanRetry())
        {
            throw new InvalidOperationException("Task cannot be retried");
        }

        Status = SendTaskStatus.PendingSend;
        IncrementRetryCount();
    }

    public void Cancel()
    {
        if (Status == SendTaskStatus.Sent)
        {
            throw new InvalidOperationException("Sent tasks cannot be cancelled");
        }

        Status = SendTaskStatus.Cancelled;
    }
}
