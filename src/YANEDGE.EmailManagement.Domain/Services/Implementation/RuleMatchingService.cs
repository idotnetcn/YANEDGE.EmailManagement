using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using MailMessageEntity = YANEDGE.EmailManagement.Domain.MailMessage.MailMessage;
using YANEDGE.EmailManagement.Domain.Rule;
using YANEDGE.EmailManagement.Enums;
using YANEDGE.EmailManagement.Domain.Services;

namespace YANEDGE.EmailManagement.Services.Implementation;

/// <summary>
/// 规则匹配执行服务实现
/// </summary>
public class RuleMatchingService : IRuleMatchingService, ITransientDependency
{
    public async Task<bool> IsMatchAsync(MailRule rule, MailMessageEntity message)
    {
        if (rule == null || message == null)
        {
            return false;
        }

        if (!rule.IsActive)
        {
            return false;
        }

        if (rule.Conditions == null || rule.Conditions.Count == 0)
        {
            return true; // 无条件则默认匹配
        }

        // 所有条件都必须满足（AND逻辑）
        foreach (var condition in rule.Conditions)
        {
            if (!await EvaluateConditionAsync(condition, message))
            {
                return false;
            }
        }

        return true;
    }

    public async Task<RuleExecutionResult> ExecuteRuleAsync(MailRule rule, MailMessageEntity message)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new RuleExecutionResult
        {
            RuleId = rule.Id,
            RuleName = rule.Name
        };

        try
        {
            // 检查是否匹配
            result.IsMatched = await IsMatchAsync(rule, message);

            if (!result.IsMatched)
            {
                result.IsSuccess = true;
                return result;
            }

            // 执行所有动作
            if (rule.Actions != null && rule.Actions.Count > 0)
            {
                foreach (var action in rule.Actions)
                {
                    var actionResult = await ExecuteActionAsync(action, message);
                    result.ActionResults[action.ActionType.ToString()] = actionResult;
                }
            }

            result.IsSuccess = true;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            stopwatch.Stop();
            result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
        }

        return result;
    }

    public async Task<List<RuleExecutionResult>> ExecuteMatchingRulesAsync(List<MailRule> rules, MailMessageEntity message)
    {
        var results = new List<RuleExecutionResult>();

        if (rules == null || rules.Count == 0)
        {
            return results;
        }

        // 按优先级排序
        var sortedRules = rules.OrderBy(r => r.Priority).ToList();

        foreach (var rule in sortedRules)
        {
            var result = await ExecuteRuleAsync(rule, message);
            results.Add(result);
        }

        return results;
    }

    private Task<bool> EvaluateConditionAsync(RuleCondition condition, MailMessageEntity message)
    {
        try
        {
            switch (condition.ConditionType)
            {
                case RuleConditionType.SenderAddress:
                    return Task.FromResult(message.FromAddress != null &&
                        message.FromAddress.Contains(condition.Value, StringComparison.OrdinalIgnoreCase));

                case RuleConditionType.SenderDomain:
                    if (message.FromAddress != null && message.FromAddress.Contains('@'))
                    {
                        var domain = message.FromAddress.Split('@')[1];
                        return Task.FromResult(domain.Equals(condition.Value, StringComparison.OrdinalIgnoreCase));
                    }
                    return Task.FromResult(false);

                case RuleConditionType.SubjectContains:
                    return Task.FromResult(message.Subject != null &&
                        message.Subject.Contains(condition.Value, StringComparison.OrdinalIgnoreCase));

                case RuleConditionType.BodyContains:
                    var body = message.SanitizedHtmlBody ?? message.TextBody ?? string.Empty;
                    return Task.FromResult(body.Contains(condition.Value, StringComparison.OrdinalIgnoreCase));

                case RuleConditionType.HasAttachment:
                    var hasAttachment = bool.TryParse(condition.Value, out var expected) && expected;
                    return Task.FromResult(message.HasAttachment == hasAttachment);

                default:
                    return Task.FromResult(false);
            }
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private Task<object> ExecuteActionAsync(RuleAction action, MailMessageEntity message)
    {
        try
        {
            switch (action.ActionType)
            {
                case RuleActionType.AddLabel:
                    // 实际实现需要调用标签服务
                    return Task.FromResult<object>(new { Action = "AddLabel", Parameters = action.Parameters });

                case RuleActionType.AutoAssign:
                    // 实际实现需要调用分派服务
                    return Task.FromResult<object>(new { Action = "AutoAssign", Parameters = action.Parameters });

                case RuleActionType.MarkImportant:
                    message.SetImportance(1);
                    return Task.FromResult<object>(new { Action = "MarkImportant", Success = true });

                case RuleActionType.AutoArchive:
                    // 实际实现需要调用归档服务
                    return Task.FromResult<object>(new { Action = "AutoArchive", Parameters = action.Parameters });

                case RuleActionType.MoveToJunk:
                    // 实际实现需要调用垃圾箱服务
                    return Task.FromResult<object>(new { Action = "MoveToJunk", Parameters = action.Parameters });

                case RuleActionType.SetPriority:
                    // 实际实现需要调用优先级设置服务
                    return Task.FromResult<object>(new { Action = "SetPriority", Parameters = action.Parameters });

                default:
                    return Task.FromResult<object>(new { Action = "Unknown", Success = false });
            }
        }
        catch (Exception ex)
        {
            return Task.FromResult<object>(new { Action = action.ActionType.ToString(), Error = ex.Message });
        }
    }
}
