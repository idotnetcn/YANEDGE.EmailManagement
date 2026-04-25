using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.MailRules;

public class CreateUpdateMailRuleInput
{
    [Required, MaxLength(128)]
    public string Name { get; set; } = null!;

    public byte RuleType { get; set; }
    public int Priority { get; set; } = 100;
    public bool IsEnabled { get; set; } = true;
    public bool StopOnMatch { get; set; }
    public Guid? AppliesToAccountId { get; set; }
    public FolderType? AppliesToFolderType { get; set; }

    [MaxLength(512)]
    public string? Description { get; set; }

    public List<MailRuleConditionInput> Conditions { get; set; } = new();
    public List<MailRuleActionInput> Actions { get; set; } = new();
}

public class MailRuleConditionInput
{
    [Required, MaxLength(64)]
    public string FieldName { get; set; } = null!;

    [Required, MaxLength(32)]
    public string Operator { get; set; } = null!;

    [Required]
    public string CompareValue { get; set; } = null!;

    [MaxLength(8)]
    public string? LogicalOperator { get; set; }
    public int SortOrder { get; set; } = 1;
}

public class MailRuleActionInput
{
    [Required, MaxLength(64)]
    public string ActionType { get; set; } = null!;

    public string? ActionValue { get; set; }
    public int ActionOrder { get; set; } = 1;
}
