using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.Approval;

/// <summary>
/// 邮件审批仓储接口
/// </summary>
public interface IMailApprovalRepository : IRepository<MailApproval, Guid>
{
    /// <summary>
    /// 查询待审批列表
    /// </summary>
    Task<List<MailApproval>> GetPendingApprovalsAsync(
        Guid approverId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按业务对象查询审批单
    /// </summary>
    Task<MailApproval?> FindByBusinessIdAsync(
        string businessType,
        Guid businessId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按状态查询
    /// </summary>
    Task<List<MailApproval>> GetByStatusAsync(
        ApprovalStatus status,
        CancellationToken cancellationToken = default);
}
