using System;
using System.Collections.Generic;
using YANEDGE.EmailManagement.Enums;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.MailRules;

public class MailRuleDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public byte RuleType { get; set; }
    public int Priority { get; set; }
    public bool IsEnabled { get; set; }
    public bool StopOnMatch { get; set; }
    public Guid? AppliesToAccountId { get; set; }
    public FolderType? AppliesToFolderType { get; set; }
    public string? Description { get; set; }
    public List<MailRuleConditionDto> Conditions { get; set; } = new();
    public List<MailRuleActionDto> Actions { get; set; } = new();
}

public class MailRuleConditionDto
{
    public Guid Id { get; set; }
    public string FieldName { get; set; } = null!;
    public string Operator { get; set; } = null!;
    public string CompareValue { get; set; } = null!;
    public string? LogicalOperator { get; set; }
    public int SortOrder { get; set; }
}

public class MailRuleActionDto
{
    public Guid Id { get; set; }
    public string ActionType { get; set; } = null!;
    public string? ActionValue { get; set; }
    public int ActionOrder { get; set; }
}
