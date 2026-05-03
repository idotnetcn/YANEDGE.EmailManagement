using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailMessageEntity = YANEDGE.EmailManagement.Domain.MailMessage.MailMessage;
using YANEDGE.EmailManagement.Domain.Rule;

namespace YANEDGE.EmailManagement.Domain.Services;

/// <summary>
/// 规则匹配执行服务接口
/// </summary>
public interface IRuleMatchingService
{
    /// <summary>
    /// 检查邮件是否匹配规则
    /// </summary>
    /// <param name="rule">规则</param>
    /// <param name="message">邮件消息</param>
    /// <returns>是否匹配</returns>
    Task<bool> IsMatchAsync(MailRule rule, MailMessageEntity message);

    /// <summary>
    /// 执行规则动作
    /// </summary>
    /// <param name="rule">规则</param>
    /// <param name="message">邮件消息</param>
    /// <returns>执行结果</returns>
    Task<RuleExecutionResult> ExecuteRuleAsync(MailRule rule, MailMessageEntity message);

    /// <summary>
    /// 批量执行匹配的规则
    /// </summary>
    /// <param name="rules">规则列表</param>
    /// <param name="message">邮件消息</param>
    /// <returns>执行结果列表</returns>
    Task<List<RuleExecutionResult>> ExecuteMatchingRulesAsync(List<MailRule> rules, MailMessageEntity message);
}

/// <summary>
/// 规则执行结果
/// </summary>
public class RuleExecutionResult
{
    public Guid RuleId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public bool IsMatched { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public long ExecutionTimeMs { get; set; }
    public Dictionary<string, object> ActionResults { get; set; } = new();
}
