using System;
using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Transport;

public class MailSendTask : AggregateRoot<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailAccountId { get; private set; }
    public Guid? MailMessageId { get; private set; }
    public string TaskNo { get; private set; } = null!;
    public SendStatus SendStatus { get; private set; }
    public int RetryCount { get; private set; }
    public int MaxRetryCount { get; set; } = 3;
    public DateTime? ScheduledSendTime { get; set; }
    public DateTime? LastSendTime { get; private set; }
    public DateTime? NextRetryTime { get; private set; }
    public string? LastErrorCode { get; private set; }
    public string? LastErrorMessage { get; private set; }
    public byte TriggeredByType { get; set; }
    public string? ExternalRequestId { get; set; }
    public string? ExternalSourceSystem { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public Guid? TriggeredByUserId { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public Guid? CancelledByUserId { get; set; }
    public Guid? OperatorUserId { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }

    protected MailSendTask() { }

    public MailSendTask(Guid id, Guid mailAccountId, string taskNo, Guid? mailMessageId = null)
    {
        Id = id;
        MailAccountId = mailAccountId;
        TaskNo = taskNo;
        MailMessageId = mailMessageId;
        SendStatus = SendStatus.Pending;
        CreationTime = DateTime.UtcNow;
    }

    public void MarkSending()
    {
        SendStatus = SendStatus.Sending;
        LastSendTime = DateTime.UtcNow;
    }

    public void MarkSucceeded()
    {
        SendStatus = SendStatus.Succeeded;
    }

    public void MarkFailed(string? errorCode, string? errorMessage, int retryDelaySeconds = 300)
    {
        SendStatus = SendStatus.Failed;
        LastErrorCode = errorCode;
        LastErrorMessage = errorMessage;
        RetryCount++;
        if (RetryCount < MaxRetryCount)
        {
            NextRetryTime = DateTime.UtcNow.AddSeconds(retryDelaySeconds);
            SendStatus = SendStatus.Pending;
        }
    }

    public void Cancel(Guid cancelledBy)
    {
        SendStatus = SendStatus.Cancelled;
        CancelledByUserId = cancelledBy;
    }
}
