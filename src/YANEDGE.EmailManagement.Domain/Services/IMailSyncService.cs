namespace YANEDGE.EmailManagement.Domain.Services;

/// <summary>
/// 邮件同步服务接口
/// </summary>
public interface IMailSyncService
{
    /// <summary>
    /// 触发邮件同步任务
    /// </summary>
    Task<MailSyncJobResult> TriggerSyncAsync(Guid mailAccountId);

    /// <summary>
    /// 执行实际的邮件同步
    /// </summary>
    Task ExecuteSyncAsync(Guid mailAccountId, Guid jobId);
}

/// <summary>
/// 邮件同步任务结果
/// </summary>
public class MailSyncJobResult
{
    /// <summary>
    /// 任务ID
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// Hangfire后台任务ID
    /// </summary>
    public string? BackgroundJobId { get; set; }

    /// <summary>
    /// 是否已接受
    /// </summary>
    public bool Accepted { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 当前状态
    /// </summary>
    public string Status { get; set; } = MailSyncExecutionStatus.Pending;
}

/// <summary>
/// 邮件同步状态快照
/// </summary>
public class MailSyncStatusSnapshot
{
    public Guid JobId { get; set; }

    public Guid MailAccountId { get; set; }

    public string Status { get; set; } = MailSyncExecutionStatus.Pending;

    public string? Message { get; set; }

    public string? BackgroundJobId { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int RetrievedCount { get; set; }

    public int SavedCount { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime ExpiresAt { get; set; }
}

public static class MailSyncExecutionStatus
{
    public const string Pending = "Pending";
    public const string Queued = "Queued";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
    public const string Skipped = "Skipped";
}
