using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.Approval;

/// <summary>
/// 邮件审批聚合根
/// </summary>
public class MailApproval : AggregateRoot<Guid>
{
    /// <summary>
    /// 业务类型(发件任务等)
    /// </summary>
    public string BusinessType { get; private set; }

    /// <summary>
    /// 业务对象ID
    /// </summary>
    public Guid BusinessId { get; private set; }

    /// <summary>
    /// 申请人ID
    /// </summary>
    public Guid ApplicantId { get; private set; }

    /// <summary>
    /// 审批状态
    /// </summary>
    public ApprovalStatus Status { get; private set; }

    /// <summary>
    /// 当前审批人ID
    /// </summary>
    public Guid? CurrentApproverId { get; private set; }

    /// <summary>
    /// 提交时间
    /// </summary>
    public DateTime SubmittedAt { get; private set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// 业务快照(JSON)
    /// </summary>
    public string? BusinessSnapshot { get; private set; }

    private MailApproval()
    {
        // For ORM
        BusinessType = string.Empty;
    }

    public MailApproval(
        Guid id,
        string businessType,
        Guid businessId,
        Guid applicantId,
        Guid currentApproverId) : base(id)
    {
        BusinessType = businessType;
        BusinessId = businessId;
        ApplicantId = applicantId;
        CurrentApproverId = currentApproverId;
        Status = ApprovalStatus.Pending;
        SubmittedAt = DateTime.UtcNow;
    }

    public void SetBusinessSnapshot(string snapshot)
    {
        BusinessSnapshot = snapshot;
    }

    public void Approve(Guid approverId, string comment)
    {
        if (Status != ApprovalStatus.Pending)
        {
            throw new InvalidOperationException("Only pending approvals can be approved");
        }

        if (CurrentApproverId != approverId)
        {
            throw new InvalidOperationException("Only current approver can approve");
        }

        Status = ApprovalStatus.Approved;
        CompletedAt = DateTime.UtcNow;
    }

    public void Reject(Guid approverId, string comment)
    {
        if (Status != ApprovalStatus.Pending)
        {
            throw new InvalidOperationException("Only pending approvals can be rejected");
        }

        if (CurrentApproverId != approverId)
        {
            throw new InvalidOperationException("Only current approver can reject");
        }

        Status = ApprovalStatus.Rejected;
        CompletedAt = DateTime.UtcNow;
    }

    public void Withdraw()
    {
        if (Status != ApprovalStatus.Pending)
        {
            throw new InvalidOperationException("Only pending approvals can be withdrawn");
        }

        Status = ApprovalStatus.Withdrawn;
        CompletedAt = DateTime.UtcNow;
    }
}
