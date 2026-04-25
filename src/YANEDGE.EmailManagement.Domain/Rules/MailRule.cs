using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Rules;

public class MailRule : FullAuditedAggregateRoot<Guid>
{
    public Guid? TenantId { get; set; }
    public string Name { get; set; } = null!;
    public byte RuleType { get; set; }
    public int Priority { get; set; } = 100;
    public bool IsEnabled { get; set; } = true;
    public bool StopOnMatch { get; set; }
    public Guid? AppliesToAccountId { get; set; }
    public FolderType? AppliesToFolderType { get; set; }
    public string? Description { get; set; }
    public string? ExtraProperties { get; set; }

    public virtual ICollection<MailRuleCondition> Conditions { get; private set; } = new List<MailRuleCondition>();
    public virtual ICollection<MailRuleAction> Actions { get; private set; } = new List<MailRuleAction>();

    protected MailRule() { }

    public MailRule(Guid id, string name, byte ruleType)
    {
        Id = id;
        Name = name;
        RuleType = ruleType;
    }
}
