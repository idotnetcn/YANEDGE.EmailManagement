using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Rule;

/// <summary>
/// 邮件规则聚合根
/// </summary>
public class MailRule : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 规则名称
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 规则描述
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// 优先级 (数值越大优先级越高)
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// 适用的邮箱账号ID列表 (空表示适用所有邮箱)
    /// </summary>
    public List<Guid> ApplicableMailAccountIds { get; private set; }

    /// <summary>
    /// 规则条件集合
    /// </summary>
    public List<RuleCondition> Conditions { get; private set; }

    /// <summary>
    /// 规则动作集合
    /// </summary>
    public List<RuleAction> Actions { get; private set; }

    /// <summary>
    /// 执行次数统计
    /// </summary>
    public int ExecutionCount { get; private set; }

    /// <summary>
    /// 最后执行时间
    /// </summary>
    public DateTime? LastExecutedAt { get; private set; }

    protected MailRule()
    {
        Name = string.Empty;
        ApplicableMailAccountIds = new List<Guid>();
        Conditions = new List<RuleCondition>();
        Actions = new List<RuleAction>();
    }

    public MailRule(
        Guid id,
        string name,
        int priority = 0,
        string? description = null
    ) : base(id)
    {
        Name = name;
        Description = description;
        Priority = priority;
        IsActive = true;
        ApplicableMailAccountIds = new List<Guid>();
        Conditions = new List<RuleCondition>();
        Actions = new List<RuleAction>();
        ExecutionCount = 0;
    }

    public void Update(string name, int priority, string? description = null)
    {
        Name = name;
        Priority = priority;
        Description = description;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void AddCondition(RuleConditionType conditionType, string value)
    {
        Conditions.Add(new RuleCondition(conditionType, value));
    }

    public void AddAction(RuleActionType actionType, string? parameters = null)
    {
        Actions.Add(new RuleAction(actionType, parameters));
    }

    public void ClearConditions()
    {
        Conditions.Clear();
    }

    public void ClearActions()
    {
        Actions.Clear();
    }

    public void RecordExecution()
    {
        ExecutionCount++;
        LastExecutedAt = DateTime.UtcNow;
    }

    public void SetApplicableMailAccounts(List<Guid> mailAccountIds)
    {
        ApplicableMailAccountIds = mailAccountIds ?? new List<Guid>();
    }
}

/// <summary>
/// 规则条件值对象
/// </summary>
public class RuleCondition
{
    /// <summary>
    /// 条件类型
    /// </summary>
    public RuleConditionType ConditionType { get; private set; }

    /// <summary>
    /// 条件值
    /// </summary>
    public string Value { get; private set; }

    protected RuleCondition()
    {
        Value = string.Empty;
    }

    public RuleCondition(RuleConditionType conditionType, string value)
    {
        ConditionType = conditionType;
        Value = value;
    }
}

/// <summary>
/// 规则动作值对象
/// </summary>
public class RuleAction
{
    /// <summary>
    /// 动作类型
    /// </summary>
    public RuleActionType ActionType { get; private set; }

    /// <summary>
    /// 动作参数 (JSON格式)
    /// </summary>
    public string? Parameters { get; private set; }

    protected RuleAction()
    {
    }

    public RuleAction(RuleActionType actionType, string? parameters = null)
    {
        ActionType = actionType;
        Parameters = parameters;
    }
}
