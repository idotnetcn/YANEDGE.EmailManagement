using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Statistics;

public class MailProcessingStatisticsDaily : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime StatDate { get; private set; }
    public int ProcessedCount { get; set; }
    public int AssignedCount { get; set; }
    public int ApprovedCount { get; set; }
    public int AvgResponseTimeSeconds { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }

    protected MailProcessingStatisticsDaily() { }

    public MailProcessingStatisticsDaily(Guid id, DateTime statDate, Guid? userId = null)
    {
        Id = id;
        StatDate = statDate.Date;
        UserId = userId;
        CreationTime = DateTime.UtcNow;
    }
}
