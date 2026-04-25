using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Statistics;

public class MailStatisticsDaily : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid? MailAccountId { get; set; }
    public DateTime StatDate { get; private set; }
    public int ReceivedCount { get; set; }
    public int SentCount { get; set; }
    public int FailedSendCount { get; set; }
    public int SpamCount { get; set; }
    public long TotalSizeBytes { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }

    protected MailStatisticsDaily() { }

    public MailStatisticsDaily(Guid id, DateTime statDate, Guid? mailAccountId = null)
    {
        Id = id;
        StatDate = statDate.Date;
        MailAccountId = mailAccountId;
        CreationTime = DateTime.UtcNow;
    }
}
