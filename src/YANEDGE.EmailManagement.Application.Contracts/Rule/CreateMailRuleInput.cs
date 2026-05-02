using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YANEDGE.EmailManagement.Rule;

public class CreateMailRuleInput
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    public int Priority { get; set; }

    public List<Guid> ApplicableMailAccountIds { get; set; } = new();

    public List<RuleConditionDto> Conditions { get; set; } = new();

    public List<RuleActionDto> Actions { get; set; } = new();
}
