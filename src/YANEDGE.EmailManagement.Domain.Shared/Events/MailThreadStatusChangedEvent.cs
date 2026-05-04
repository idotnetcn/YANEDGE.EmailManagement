using System;
using Volo.Abp.EventBus;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.Shared.Events;

/// <summary>
/// 线程状态变更事件
/// Event triggered when mail thread status changes
/// </summary>
[EventName("EmailManagement.MailThread.StatusChanged")]
public class MailThreadStatusChangedEvent 
{
    public Guid ThreadId { get; set; }
    public Guid MailAccountId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public ThreadStatus OldStatus { get; set; }
    public ThreadStatus NewStatus { get; set; }
    public Guid? CurrentAssigneeId { get; set; }
    public DateTime ChangedTime { get; set; }
}
