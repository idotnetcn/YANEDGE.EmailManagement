using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace YANEDGE.EmailManagement.Domain.Contact;

/// <summary>
/// 联系人聚合根
/// </summary>
public class MailContact : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 邮箱地址
    /// </summary>
    public string EmailAddress { get; private set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// 公司名称
    /// </summary>
    public string? CompanyName { get; private set; }

    /// <summary>
    /// 职位
    /// </summary>
    public string? JobTitle { get; private set; }

    /// <summary>
    /// 所属客户ID (来自CRM)
    /// </summary>
    public string? CustomerId { get; private set; }

    /// <summary>
    /// 所属供应商ID (来自ERP)
    /// </summary>
    public string? SupplierId { get; private set; }

    /// <summary>
    /// 来源系统 (Manual/CRM/ERP)
    /// </summary>
    public string Source { get; private set; }

    /// <summary>
    /// 外部系统ID
    /// </summary>
    public string? ExternalId { get; private set; }

    /// <summary>
    /// 最近联系时间
    /// </summary>
    public DateTime? LastContactedAt { get; private set; }

    /// <summary>
    /// 关联邮件数量
    /// </summary>
    public int MailCount { get; private set; }

    /// <summary>
    /// 是否已验证
    /// </summary>
    public bool IsVerified { get; private set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Notes { get; private set; }

    protected MailContact()
    {
        Name = string.Empty;
        EmailAddress = string.Empty;
        Source = "Manual";
    }

    public MailContact(
        Guid id,
        string name,
        string emailAddress,
        string source = "Manual",
        string? phoneNumber = null,
        string? companyName = null,
        string? jobTitle = null,
        string? customerId = null,
        string? supplierId = null,
        string? externalId = null,
        string? notes = null
    ) : base(id)
    {
        Name = name;
        EmailAddress = emailAddress;
        Source = source;
        PhoneNumber = phoneNumber;
        CompanyName = companyName;
        JobTitle = jobTitle;
        CustomerId = customerId;
        SupplierId = supplierId;
        ExternalId = externalId;
        Notes = notes;
        IsActive = true;
        IsVerified = false;
        MailCount = 0;
    }

    public void Update(
        string name,
        string? phoneNumber = null,
        string? companyName = null,
        string? jobTitle = null,
        string? notes = null
    )
    {
        Name = name;
        PhoneNumber = phoneNumber;
        CompanyName = companyName;
        JobTitle = jobTitle;
        Notes = notes;
    }

    public void UpdateLastContactedAt(DateTime contactedAt)
    {
        LastContactedAt = contactedAt;
    }

    public void IncrementMailCount()
    {
        MailCount++;
    }

    public void Verify()
    {
        IsVerified = true;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void LinkToCustomer(string customerId)
    {
        CustomerId = customerId;
    }

    public void LinkToSupplier(string supplierId)
    {
        SupplierId = supplierId;
    }
}
