using System;
using Volo.Abp.Domain.Entities;

namespace YANEDGE.EmailManagement.Label;

/// <summary>
/// 邮件-标签关联实体
/// </summary>
public class MailMessageLabel : Entity
{
    /// <summary>
    /// 邮件ID
    /// </summary>
    public Guid MailMessageId { get; private set; }

    /// <summary>
    /// 标签ID
    /// </summary>
    public Guid LabelId { get; private set; }

    /// <summary>
    /// 添加时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 添加人
    /// </summary>
    public Guid? CreatorId { get; private set; }

    protected MailMessageLabel()
    {
    }

    public MailMessageLabel(Guid mailMessageId, Guid labelId, Guid? creatorId = null)
    {
        MailMessageId = mailMessageId;
        LabelId = labelId;
        CreatorId = creatorId;
        CreatedAt = DateTime.UtcNow;
    }

    public override object[] GetKeys()
    {
        return new object[] { MailMessageId, LabelId };
    }
}
