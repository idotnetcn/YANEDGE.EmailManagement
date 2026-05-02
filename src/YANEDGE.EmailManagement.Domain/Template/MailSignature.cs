using System;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Template;

/// <summary>
/// 邮件签名聚合根
/// </summary>
public class MailSignature : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 签名名称
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 签名内容 (HTML)
    /// </summary>
    public string Content { get; private set; }

    /// <summary>
    /// 纯文本签名
    /// </summary>
    public string? PlainTextContent { get; private set; }

    /// <summary>
    /// 签名作用域
    /// </summary>
    public SignatureScope Scope { get; private set; }

    /// <summary>
    /// 所属用户ID (个人签名时使用)
    /// </summary>
    public Guid? OwnerUserId { get; private set; }

    /// <summary>
    /// 所属组织ID (部门签名时使用)
    /// </summary>
    public Guid? OwnerOrganizationId { get; private set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// 是否为默认签名
    /// </summary>
    public bool IsDefault { get; private set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; private set; }

    protected MailSignature()
    {
        Name = string.Empty;
        Content = string.Empty;
    }

    public MailSignature(
        Guid id,
        string name,
        string content,
        SignatureScope scope,
        Guid? ownerUserId = null,
        Guid? ownerOrganizationId = null,
        string? plainTextContent = null,
        string? description = null
    ) : base(id)
    {
        Name = name;
        Content = content;
        Scope = scope;
        OwnerUserId = ownerUserId;
        OwnerOrganizationId = ownerOrganizationId;
        PlainTextContent = plainTextContent;
        Description = description;
        IsActive = true;
        IsDefault = false;
        SortOrder = 0;
    }

    public void Update(
        string name,
        string content,
        string? plainTextContent = null,
        string? description = null
    )
    {
        Name = name;
        Content = content;
        PlainTextContent = plainTextContent;
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

    public void SetAsDefault()
    {
        IsDefault = true;
    }

    public void UnsetAsDefault()
    {
        IsDefault = false;
    }

    public void SetSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }
}
