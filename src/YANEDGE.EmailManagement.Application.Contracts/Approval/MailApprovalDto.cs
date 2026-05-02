using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Application.Contracts.Approval;

/// <summary>
/// 审批DTO
/// </summary>
public class MailApprovalDto
{
    public Guid Id { get; set; }
    public string BusinessType { get; set; } = string.Empty;
    public Guid BusinessId { get; set; }
    public Guid ApplicantId { get; set; }
    public ApprovalStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public Guid? CurrentApproverId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
