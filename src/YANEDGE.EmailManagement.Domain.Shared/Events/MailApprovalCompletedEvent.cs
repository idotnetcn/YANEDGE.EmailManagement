using System;
using Volo.Abp.EventBus;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.Shared.Events;

/// <summary>
/// 审批完成事件
/// Event triggered when approval is completed
/// </summary>
[EventName("EmailManagement.MailApproval.Completed")]
public class MailApprovalCompletedEvent 
{
    public Guid ApprovalId { get; set; }
    public string BusinessType { get; set; } = string.Empty;
    public Guid BusinessId { get; set; }
    public ApprovalStatus Status { get; set; }
    public Guid? ApproverId { get; set; }
    public string? ApprovalComments { get; set; }
    public DateTime CompletedTime { get; set; }
}
