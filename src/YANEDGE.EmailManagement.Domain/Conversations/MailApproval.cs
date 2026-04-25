using System;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Conversations;

public class MailApproval : AuditedAggregateRoot<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailMessageId { get; private set; }
    public string ApprovalCode { get; private set; } = null!;
    public string? FlowName { get; set; }
    public string? NodeName { get; set; }
    public Guid? ApproverUserId { get; set; }
    public ApprovalStatus ApprovalStatus { get; private set; }
    public Guid SubmittedUserId { get; private set; }
    public DateTime SubmittedTime { get; private set; }
    public DateTime? ProcessedTime { get; private set; }
    public string? Comment { get; set; }

    protected MailApproval() { }

    public MailApproval(Guid id, Guid mailMessageId, string approvalCode, Guid submittedUserId)
    {
        Id = id;
        MailMessageId = mailMessageId;
        ApprovalCode = approvalCode;
        SubmittedUserId = submittedUserId;
        SubmittedTime = DateTime.UtcNow;
        ApprovalStatus = ApprovalStatus.Pending;
    }

    public void Approve(string? comment)
    {
        ApprovalStatus = ApprovalStatus.Approved;
        Comment = comment;
        ProcessedTime = DateTime.UtcNow;
    }

    public void Reject(string? comment)
    {
        ApprovalStatus = ApprovalStatus.Rejected;
        Comment = comment;
        ProcessedTime = DateTime.UtcNow;
    }

    public void Withdraw()
    {
        ApprovalStatus = ApprovalStatus.Withdrawn;
        ProcessedTime = DateTime.UtcNow;
    }
}
