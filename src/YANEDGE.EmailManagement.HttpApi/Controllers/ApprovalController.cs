using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using YANEDGE.EmailManagement.Application.Contracts.Approval;

namespace YANEDGE.EmailManagement.HttpApi.Controllers;

[Route("api/mail-management/v1/approvals")]
public class ApprovalController : AbpControllerBase
{
    private readonly IApprovalAppService _approvalAppService;

    public ApprovalController(IApprovalAppService approvalAppService)
    {
        _approvalAppService = approvalAppService;
    }

    [HttpGet("pending")]
    public Task<List<MailApprovalDto>> GetPendingApprovalsAsync()
    {
        return _approvalAppService.GetPendingApprovalsAsync();
    }

    [HttpGet("{id}")]
    public Task<MailApprovalDto> GetAsync(Guid id)
    {
        return _approvalAppService.GetAsync(id);
    }

    [HttpPost("{id}/approve")]
    public Task<MailApprovalDto> ApproveAsync(Guid id, [FromBody] ApproveInput input)
    {
        return _approvalAppService.ApproveAsync(id, input);
    }

    [HttpPost("{id}/reject")]
    public Task<MailApprovalDto> RejectAsync(Guid id, [FromBody] RejectInput input)
    {
        return _approvalAppService.RejectAsync(id, input);
    }

    [HttpPost("{id}/withdraw")]
    public Task<MailApprovalDto> WithdrawAsync(Guid id)
    {
        return _approvalAppService.WithdrawAsync(id);
    }
}
