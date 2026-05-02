using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Application.Contracts.MailMessage;

/// <summary>
/// 邮件消息DTO
/// </summary>
public class MailMessageDto
{
    public Guid Id { get; set; }
    public Guid MailAccountId { get; set; }
    public Guid? ThreadId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public MailDirection Direction { get; set; }
    public string DirectionName { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string? FromDisplayName { get; set; }
    public DateTime? ReceivedTime { get; set; }
    public DateTime? SentTime { get; set; }
    public bool HasAttachment { get; set; }
    public bool IsRead { get; set; }
    public int Importance { get; set; }
}
