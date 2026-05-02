using System;
using System.Collections.Generic;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Rule;

public class RuleConditionDto
{
    public RuleConditionType ConditionType { get; set; }

    public string Value { get; set; } = null!;
}
