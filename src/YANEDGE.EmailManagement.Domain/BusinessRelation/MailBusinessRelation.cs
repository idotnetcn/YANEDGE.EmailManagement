using System;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.BusinessRelation;

/// <summary>
/// 邮件业务对象关联实体
/// </summary>
public class MailBusinessRelation : CreationAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 邮件ID
    /// </summary>
    public Guid MailMessageId { get; private set; }

    /// <summary>
    /// 线程ID
    /// </summary>
    public Guid? ThreadId { get; private set; }

    /// <summary>
    /// 业务对象类型
    /// </summary>
    public BusinessObjectType BusinessObjectType { get; private set; }

    /// <summary>
    /// 业务对象ID (外部系统ID)
    /// </summary>
    public string BusinessObjectId { get; private set; }

    /// <summary>
    /// 业务对象名称
    /// </summary>
    public string? BusinessObjectName { get; private set; }

    /// <summary>
    /// 业务对象编号
    /// </summary>
    public string? BusinessObjectCode { get; private set; }

    /// <summary>
    /// 是否主关联
    /// </summary>
    public bool IsPrimary { get; private set; }

    /// <summary>
    /// 关联来源 (Auto/Manual/API)
    /// </summary>
    public string RelationSource { get; private set; }

    /// <summary>
    /// 外部系统名称
    /// </summary>
    public string? ExternalSystem { get; private set; }

    /// <summary>
    /// 关联备注
    /// </summary>
    public string? Notes { get; private set; }

    protected MailBusinessRelation()
    {
        BusinessObjectId = string.Empty;
        RelationSource = "Manual";
    }

    public MailBusinessRelation(
        Guid id,
        Guid mailMessageId,
        Guid? threadId,
        BusinessObjectType businessObjectType,
        string businessObjectId,
        string relationSource = "Manual",
        string? businessObjectName = null,
        string? businessObjectCode = null,
        bool isPrimary = false,
        string? externalSystem = null,
        string? notes = null
    ) : base(id)
    {
        MailMessageId = mailMessageId;
        ThreadId = threadId;
        BusinessObjectType = businessObjectType;
        BusinessObjectId = businessObjectId;
        RelationSource = relationSource;
        BusinessObjectName = businessObjectName;
        BusinessObjectCode = businessObjectCode;
        IsPrimary = isPrimary;
        ExternalSystem = externalSystem;
        Notes = notes;
    }

    public void Update(
        string? businessObjectName = null,
        string? businessObjectCode = null,
        bool? isPrimary = null,
        string? notes = null
    )
    {
        if (businessObjectName != null)
            BusinessObjectName = businessObjectName;

        if (businessObjectCode != null)
            BusinessObjectCode = businessObjectCode;

        if (isPrimary.HasValue)
            IsPrimary = isPrimary.Value;

        if (notes != null)
            Notes = notes;
    }

    public void SetAsPrimary()
    {
        IsPrimary = true;
    }

    public void UnsetAsPrimary()
    {
        IsPrimary = false;
    }
}
