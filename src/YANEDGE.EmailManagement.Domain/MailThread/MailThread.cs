using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.MailThread;

/// <summary>
/// 邮件线程聚合根
/// </summary>
public class MailThread : AggregateRoot<Guid>
{
    /// <summary>
    /// 邮箱账号ID
    /// </summary>
    public Guid MailAccountId { get; private set; }

    /// <summary>
    /// 线程主题
    /// </summary>
    public string Subject { get; private set; }

    /// <summary>
    /// 标准化主题
    /// </summary>
    public string NormalizedSubject { get; private set; }

    /// <summary>
    /// 线程状态
    /// </summary>
    public ThreadStatus Status { get; private set; }

    /// <summary>
    /// 当前负责人类型
    /// </summary>
    public AssigneeType? CurrentAssigneeType { get; private set; }

    /// <summary>
    /// 当前负责人ID
    /// </summary>
    public Guid? CurrentAssigneeId { get; private set; }

    /// <summary>
    /// 最近消息时间
    /// </summary>
    public DateTime LatestMessageTime { get; private set; }

    /// <summary>
    /// 邮件数量
    /// </summary>
    public int MessageCount { get; private set; }

    /// <summary>
    /// 是否有附件
    /// </summary>
    public bool HasAttachment { get; private set; }

    /// <summary>
    /// 优先级
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    private MailThread()
    {
        // For ORM
        Subject = string.Empty;
        NormalizedSubject = string.Empty;
    }

    public MailThread(
        Guid id,
        Guid mailAccountId,
        string subject,
        DateTime latestMessageTime) : base(id)
    {
        MailAccountId = mailAccountId;
        Subject = subject;
        NormalizedSubject = NormalizeSubject(subject);
        Status = ThreadStatus.Pending;
        LatestMessageTime = latestMessageTime;
        MessageCount = 0;
        HasAttachment = false;
        Priority = 0;
        CreatedAt = DateTime.UtcNow;
    }

    private static string NormalizeSubject(string subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            return string.Empty;
        }

        // Remove Re:, Fwd:, etc.
        var normalized = subject.Trim();
        var prefixes = new[] { "Re:", "RE:", "Fwd:", "FWD:", "Fw:", "FW:" };

        foreach (var prefix in prefixes)
        {
            while (normalized.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(prefix.Length).Trim();
            }
        }

        return normalized.ToLowerInvariant();
    }

    public void Assign(AssigneeType assigneeType, Guid assigneeId)
    {
        CurrentAssigneeType = assigneeType;
        CurrentAssigneeId = assigneeId;

        if (Status == ThreadStatus.Pending)
        {
            Status = ThreadStatus.Todo;
        }
    }

    public void Claim(Guid userId)
    {
        Assign(AssigneeType.User, userId);
    }

    public void StartProcessing()
    {
        if (Status == ThreadStatus.Todo)
        {
            Status = ThreadStatus.Processing;
        }
    }

    public void Complete()
    {
        if (Status == ThreadStatus.Processing || Status == ThreadStatus.Todo)
        {
            Status = ThreadStatus.Completed;
        }
    }

    public void Archive()
    {
        if (Status == ThreadStatus.Completed)
        {
            Status = ThreadStatus.Archived;
        }
    }

    public void Close()
    {
        Status = ThreadStatus.Closed;
    }

    public void Reopen()
    {
        if (Status == ThreadStatus.Closed || Status == ThreadStatus.Archived)
        {
            Status = ThreadStatus.Processing;
        }
    }

    public void IncrementMessageCount()
    {
        MessageCount++;
    }

    public void UpdateLatestMessageTime(DateTime messageTime)
    {
        if (messageTime > LatestMessageTime)
        {
            LatestMessageTime = messageTime;
        }
    }

    public void SetHasAttachment()
    {
        HasAttachment = true;
    }

    public void UpdatePriority(int priority)
    {
        Priority = priority;
    }
}
