using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.Application.Contracts.Approval;

/// <summary>
/// 审批应用服务接口
/// </summary>
public interface IApprovalAppService : IApplicationService
{
    /// <summary>
    /// 查询待审批列表
    /// </summary>
    Task<List<MailApprovalDto>> GetPendingApprovalsAsync();

    /// <summary>
    /// 获取审批详情
    /// </summary>
    Task<MailApprovalDto> GetAsync(Guid id);

    /// <summary>
    /// 审批通过
    /// </summary>
    Task<MailApprovalDto> ApproveAsync(Guid id, ApproveInput input);

    /// <summary>
    /// 审批拒绝
    /// </summary>
    Task<MailApprovalDto> RejectAsync(Guid id, RejectInput input);

    /// <summary>
    /// 撤回审批
    /// </summary>
    Task<MailApprovalDto> WithdrawAsync(Guid id);
}

public class ApproveInput
{
    public string? Comment { get; set; }
}

public class RejectInput
{
    public string Comment { get; set; } = string.Empty;
}
