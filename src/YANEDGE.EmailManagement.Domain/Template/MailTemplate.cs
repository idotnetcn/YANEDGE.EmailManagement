using System;
using Volo.Abp.Domain.Entities.Auditing;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Template;

/// <summary>
/// 邮件模板聚合根
/// </summary>
public class MailTemplate : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 模板编码 (唯一标识)
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// 模板名称
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 模板分类
    /// </summary>
    public string? Category { get; private set; }

    /// <summary>
    /// 语言
    /// </summary>
    public string Language { get; private set; }

    /// <summary>
    /// 主题模板
    /// </summary>
    public string SubjectTemplate { get; private set; }

    /// <summary>
    /// 正文模板 (HTML)
    /// </summary>
    public string BodyTemplate { get; private set; }

    /// <summary>
    /// 纯文本正文模板
    /// </summary>
    public string? PlainTextTemplate { get; private set; }

    /// <summary>
    /// 模板状态
    /// </summary>
    public TemplateStatus Status { get; private set; }

    /// <summary>
    /// 当前版本号
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// 是否需要审批
    /// </summary>
    public bool RequiresApproval { get; private set; }

    /// <summary>
    /// 是否为默认模板
    /// </summary>
    public bool IsDefault { get; private set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; private set; }

    protected MailTemplate()
    {
        Code = string.Empty;
        Name = string.Empty;
        Language = "zh-CN";
        SubjectTemplate = string.Empty;
        BodyTemplate = string.Empty;
    }

    public MailTemplate(
        Guid id,
        string code,
        string name,
        string subjectTemplate,
        string bodyTemplate,
        string language = "zh-CN",
        string? category = null,
        bool requiresApproval = false,
        string? description = null
    ) : base(id)
    {
        Code = code;
        Name = name;
        SubjectTemplate = subjectTemplate;
        BodyTemplate = bodyTemplate;
        Language = language;
        Category = category;
        RequiresApproval = requiresApproval;
        Description = description;
        Status = TemplateStatus.Draft;
        Version = 1;
        IsDefault = false;
        SortOrder = 0;
    }

    public void Update(
        string name,
        string subjectTemplate,
        string bodyTemplate,
        string? category = null,
        string? plainTextTemplate = null,
        string? description = null
    )
    {
        Name = name;
        SubjectTemplate = subjectTemplate;
        BodyTemplate = bodyTemplate;
        Category = category;
        PlainTextTemplate = plainTextTemplate;
        Description = description;
        Version++;
    }

    public void Activate()
    {
        if (Status == TemplateStatus.Active)
            throw new InvalidOperationException("Template is already active");

        Status = TemplateStatus.Active;
    }

    public void Deactivate()
    {
        if (Status != TemplateStatus.Active)
            throw new InvalidOperationException("Only active templates can be deactivated");

        Status = TemplateStatus.Inactive;
    }

    public void Archive()
    {
        if (Status == TemplateStatus.Archived)
            throw new InvalidOperationException("Template is already archived");

        Status = TemplateStatus.Archived;
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
