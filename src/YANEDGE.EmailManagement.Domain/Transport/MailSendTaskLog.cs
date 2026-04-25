using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Transport;

public class MailSendTaskLog : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid MailSendTaskId { get; private set; }
    public int AttemptNo { get; set; }
    public DateTime ExecutionTime { get; set; }
    public byte ResultStatus { get; set; }
    public string? ProviderResponse { get; set; }
    public string? ErrorMessage { get; set; }
    public string? TraceId { get; set; }

    protected MailSendTaskLog() { }

    public MailSendTaskLog(Guid id, Guid mailSendTaskId, int attemptNo)
    {
        Id = id;
        MailSendTaskId = mailSendTaskId;
        AttemptNo = attemptNo;
        ExecutionTime = DateTime.UtcNow;
    }
}
