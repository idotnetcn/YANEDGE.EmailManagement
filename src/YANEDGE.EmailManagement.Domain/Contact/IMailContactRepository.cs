using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Contact;

/// <summary>
/// 联系人仓储接口
/// </summary>
public interface IMailContactRepository : IRepository<MailContact, Guid>
{
    /// <summary>
    /// 根据邮箱地址查找联系人
    /// </summary>
    Task<MailContact?> FindByEmailAddressAsync(
        string emailAddress,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 根据客户ID获取联系人列表
    /// </summary>
    Task<List<MailContact>> GetByCustomerIdAsync(
        string customerId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 根据供应商ID获取联系人列表
    /// </summary>
    Task<List<MailContact>> GetBySupplierIdAsync(
        string supplierId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 根据外部系统ID查找联系人
    /// </summary>
    Task<MailContact?> FindByExternalIdAsync(
        string source,
        string externalId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// 搜索联系人 (按姓名、邮箱、公司名称)
    /// </summary>
    Task<List<MailContact>> SearchAsync(
        string keyword,
        int skipCount,
        int maxResultCount,
        CancellationToken cancellationToken = default
    );
}
