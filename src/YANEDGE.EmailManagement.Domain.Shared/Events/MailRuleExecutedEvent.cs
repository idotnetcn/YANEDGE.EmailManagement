using System;
using Volo.Abp.EventBus;

namespace YANEDGE.EmailManagement.Domain.Shared.Events;

/// <summary>
/// 规则执行事件
/// Event triggered when a mail rule is executed
/// </summary>
[EventName("EmailManagement.MailRule.Executed")]
public class MailRuleExecutedEvent 
{
    public Guid RuleId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public Guid MessageId { get; set; }
    public Guid ThreadId { get; set; }
    public bool IsMatched { get; set; }
    public string? ActionsExecuted { get; set; }
    public DateTime ExecutedTime { get; set; }
}
