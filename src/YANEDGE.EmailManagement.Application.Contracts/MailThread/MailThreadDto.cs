using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Application.Contracts.MailThread;

/// <summary>
/// 线程DTO
/// </summary>
public class MailThreadDto
{
    public Guid Id { get; set; }
    public Guid MailAccountId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public ThreadStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public AssigneeType? CurrentAssigneeType { get; set; }
    public Guid? CurrentAssigneeId { get; set; }
    public DateTime LatestMessageTime { get; set; }
    public int MessageCount { get; set; }
    public bool HasAttachment { get; set; }
    public int Priority { get; set; }
    public int UnreadCount { get; set; }
}
