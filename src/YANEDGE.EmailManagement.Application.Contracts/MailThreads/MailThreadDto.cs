using System;
using YANEDGE.EmailManagement.Enums;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.MailThreads;

public class MailThreadDto : FullAuditedEntityDto<Guid>
{
    public Guid MailAccountId { get; set; }
    public string? Subject { get; set; }
    public string ThreadKey { get; set; } = null!;
    public DateTime? LatestMessageTime { get; set; }
    public string? LatestSender { get; set; }
    public int MessageCount { get; set; }
    public int UnreadCount { get; set; }
    public ThreadStatus Status { get; set; }
    public Guid? OwnerUserId { get; set; }
    public Importance Priority { get; set; }
    public bool IsStarred { get; set; }
}
