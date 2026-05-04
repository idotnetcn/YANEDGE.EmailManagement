using System;
using Volo.Abp.EventBus;

namespace YANEDGE.EmailManagement.Domain.Shared.Events;

/// <summary>
/// 审批请求事件
/// Event triggered when approval is requested
/// </summary>
[EventName("EmailManagement.MailApproval.Requested")]
public class MailApprovalRequestedEvent 
{
    public Guid ApprovalId { get; set; }
    public string BusinessType { get; set; } = string.Empty;
    public Guid BusinessId { get; set; }
    public Guid? CurrentApproverId { get; set; }
    public string? Remarks { get; set; }
    public DateTime RequestedTime { get; set; }
}
