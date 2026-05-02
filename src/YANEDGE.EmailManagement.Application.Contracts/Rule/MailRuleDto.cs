using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Rule;

public class MailRuleDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int Priority { get; set; }

    public List<Guid> ApplicableMailAccountIds { get; set; } = new();

    public List<RuleConditionDto> Conditions { get; set; } = new();

    public List<RuleActionDto> Actions { get; set; } = new();

    public int ExecutionCount { get; set; }

    public DateTime? LastExecutedAt { get; set; }
}
