using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Rules;

public class MailRuleAction : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid RuleId { get; private set; }
    public string ActionType { get; set; } = null!;
    public string? ActionValue { get; set; }
    public int ActionOrder { get; set; } = 1;
    public DateTime CreationTime { get; set; }

    protected MailRuleAction() { }

    public MailRuleAction(Guid id, Guid ruleId, string actionType)
    {
        Id = id;
        RuleId = ruleId;
        ActionType = actionType;
        CreationTime = DateTime.UtcNow;
    }
}
