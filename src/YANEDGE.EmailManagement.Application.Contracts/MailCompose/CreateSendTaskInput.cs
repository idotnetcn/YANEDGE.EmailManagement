using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Application.Contracts.MailCompose;

/// <summary>
/// 创建发件任务输入
/// </summary>
public class CreateSendTaskInput
{
    public Guid MailAccountId { get; set; }
    public Guid? ThreadId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? BodyHtml { get; set; }
    public string? BodyText { get; set; }
    public Guid? TemplateId { get; set; }
    public Guid? SignatureId { get; set; }
    public DateTime? ScheduledSendTime { get; set; }
    public List<RecipientInput> Recipients { get; set; } = new();
    public List<Guid> AttachmentIds { get; set; } = new();
    public string? ExternalBizRef { get; set; }
}

public class RecipientInput
{
    public RecipientType RecipientType { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
}
