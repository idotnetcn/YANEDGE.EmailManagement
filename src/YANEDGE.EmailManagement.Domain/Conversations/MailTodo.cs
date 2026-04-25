using System;
using Volo.Abp.Domain.Entities;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Conversations;

public class MailTodo : Entity<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid ThreadId { get; private set; }
    public Guid RelatedUserId { get; private set; }
    public TodoType TodoType { get; set; }
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public DateTime? DueTime { get; set; }
    public TodoStatus Status { get; set; } = TodoStatus.Pending;
    public DateTime? CompletedTime { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorId { get; set; }

    protected MailTodo() { }

    public MailTodo(Guid id, Guid threadId, Guid relatedUserId, TodoType todoType, string title)
    {
        Id = id;
        ThreadId = threadId;
        RelatedUserId = relatedUserId;
        TodoType = todoType;
        Title = title;
        CreationTime = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = TodoStatus.Completed;
        CompletedTime = DateTime.UtcNow;
    }
}
