using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Rules;

public class MailRuleExecutionLog : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid RuleId { get; set; }
    public Guid MailMessageId { get; set; }
    public bool HitStatus { get; set; }
    public byte ExecutionResult { get; set; }
    public string? ResultMessage { get; set; }
    public DateTime ExecutionTime { get; set; }

    protected MailRuleExecutionLog() { }

    public MailRuleExecutionLog(Guid id, Guid ruleId, Guid mailMessageId)
    {
        Id = id;
        RuleId = ruleId;
        MailMessageId = mailMessageId;
        ExecutionTime = DateTime.UtcNow;
    }
}
