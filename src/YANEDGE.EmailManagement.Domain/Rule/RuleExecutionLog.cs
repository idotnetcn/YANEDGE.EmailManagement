using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace YANEDGE.EmailManagement.Domain.Rule;

/// <summary>
/// 规则执行日志实体
/// </summary>
public class RuleExecutionLog : CreationAuditedEntity<Guid>
{
    /// <summary>
    /// 规则ID
    /// </summary>
    public Guid RuleId { get; private set; }

    /// <summary>
    /// 邮件ID
    /// </summary>
    public Guid MailMessageId { get; private set; }

    /// <summary>
    /// 线程ID
    /// </summary>
    public Guid? ThreadId { get; private set; }

    /// <summary>
    /// 是否命中
    /// </summary>
    public bool IsMatched { get; private set; }

    /// <summary>
    /// 执行结果
    /// </summary>
    public string? ExecutionResult { get; private set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// 执行耗时 (毫秒)
    /// </summary>
    public long ExecutionTimeMs { get; private set; }

    protected RuleExecutionLog()
    {
    }

    public RuleExecutionLog(
        Guid id,
        Guid ruleId,
        Guid mailMessageId,
        Guid? threadId,
        bool isMatched,
        long executionTimeMs,
        string? executionResult = null,
        string? errorMessage = null
    ) : base(id)
    {
        RuleId = ruleId;
        MailMessageId = mailMessageId;
        ThreadId = threadId;
        IsMatched = isMatched;
        ExecutionTimeMs = executionTimeMs;
        ExecutionResult = executionResult;
        ErrorMessage = errorMessage;
    }
}
