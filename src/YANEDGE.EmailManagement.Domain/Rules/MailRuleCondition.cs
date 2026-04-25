using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Rules;

public class MailRuleCondition : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid RuleId { get; private set; }
    public string FieldName { get; set; } = null!;
    public string Operator { get; set; } = null!;
    public string CompareValue { get; set; } = null!;
    public string? LogicalOperator { get; set; }
    public int SortOrder { get; set; } = 1;
    public DateTime CreationTime { get; set; }

    protected MailRuleCondition() { }

    public MailRuleCondition(Guid id, Guid ruleId, string fieldName, string op, string compareValue)
    {
        Id = id;
        RuleId = ruleId;
        FieldName = fieldName;
        Operator = op;
        CompareValue = compareValue;
        CreationTime = DateTime.UtcNow;
    }
}
