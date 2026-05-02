using System;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Rule;

public class RuleActionDto
{
    public RuleActionType ActionType { get; set; }

    public string? Parameters { get; set; }
}
