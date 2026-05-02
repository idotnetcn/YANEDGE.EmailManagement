using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace YANEDGE.EmailManagement.Label;

/// <summary>
/// 邮件标签聚合根
/// </summary>
public class MailLabel : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 标签名称
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 标签颜色 (十六进制颜色码, 如: #FF5733)
    /// </summary>
    public string Color { get; private set; }

    /// <summary>
    /// 标签描述
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 是否系统标签
    /// </summary>
    public bool IsSystemLabel { get; private set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// 所属用户ID (个人标签)
    /// </summary>
    public Guid? OwnerUserId { get; private set; }

    /// <summary>
    /// 所属组织ID (组织标签)
    /// </summary>
    public Guid? OwnerOrganizationId { get; private set; }

    protected MailLabel()
    {
        Name = string.Empty;
        Color = "#000000";
    }

    public MailLabel(
        Guid id,
        string name,
        string color,
        Guid? ownerUserId = null,
        Guid? ownerOrganizationId = null,
        string? description = null,
        bool isSystemLabel = false
    ) : base(id)
    {
        Name = name;
        Color = color;
        OwnerUserId = ownerUserId;
        OwnerOrganizationId = ownerOrganizationId;
        Description = description;
        IsSystemLabel = isSystemLabel;
        IsActive = true;
        SortOrder = 0;
    }

    public void Update(string name, string color, string? description = null)
    {
        Name = name;
        Color = color;
        Description = description;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void SetSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }
}
