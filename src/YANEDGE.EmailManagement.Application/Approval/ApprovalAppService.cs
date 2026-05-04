using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;
using YANEDGE.EmailManagement.Application.Contracts.Approval;
using YANEDGE.EmailManagement.Domain.Approval;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Permissions;

namespace YANEDGE.EmailManagement.Application.Approval;

/// <summary>
/// 审批应用服务
/// </summary>
[Authorize(EmailManagementPermissions.SendTasks.Approve)]
public class ApprovalAppService : ApplicationService, IApprovalAppService
{
    private readonly IMailApprovalRepository _mailApprovalRepository;
    private readonly IMailSendTaskRepository _mailSendTaskRepository;
    private readonly ICurrentUser _currentUser;

    public ApprovalAppService(
        IMailApprovalRepository mailApprovalRepository,
        IMailSendTaskRepository mailSendTaskRepository,
        ICurrentUser currentUser)
    {
        _mailApprovalRepository = mailApprovalRepository;
        _mailSendTaskRepository = mailSendTaskRepository;
        _currentUser = currentUser;
    }

    public async Task<List<MailApprovalDto>> GetPendingApprovalsAsync()
    {
        var userId = _currentUser.Id ?? throw new InvalidOperationException("User not authenticated");

        var approvals = await _mailApprovalRepository.GetPendingApprovalsAsync(userId);

        return approvals.Select(MapToDto).ToList();
    }

    public async Task<MailApprovalDto> GetAsync(Guid id)
    {
        var approval = await _mailApprovalRepository.GetAsync(id);

        return MapToDto(approval);
    }

    public async Task<MailApprovalDto> ApproveAsync(Guid id, ApproveInput input)
    {
        var approval = await _mailApprovalRepository.GetAsync(id);
        var userId = _currentUser.Id ?? throw new InvalidOperationException("User not authenticated");

        approval.Approve(userId, input.Comment ?? "Approved");

        await _mailApprovalRepository.UpdateAsync(approval);

        // Update related business object (e.g., send task)
        if (approval.BusinessType == "SendTask")
        {
            var sendTask = await _mailSendTaskRepository.GetAsync(approval.BusinessId);
            sendTask.ApprovalApproved();
            await _mailSendTaskRepository.UpdateAsync(sendTask);
        }

        return MapToDto(approval);
    }

    public async Task<MailApprovalDto> RejectAsync(Guid id, RejectInput input)
    {
        var approval = await _mailApprovalRepository.GetAsync(id);
        var userId = _currentUser.Id ?? throw new InvalidOperationException("User not authenticated");

        approval.Reject(userId, input.Comment);

        await _mailApprovalRepository.UpdateAsync(approval);

        // Update related business object (e.g., send task)
        if (approval.BusinessType == "SendTask")
        {
            var sendTask = await _mailSendTaskRepository.GetAsync(approval.BusinessId);
            sendTask.ApprovalRejected();
            await _mailSendTaskRepository.UpdateAsync(sendTask);
        }

        return MapToDto(approval);
    }

    public async Task<MailApprovalDto> WithdrawAsync(Guid id)
    {
        var approval = await _mailApprovalRepository.GetAsync(id);

        approval.Withdraw();

        await _mailApprovalRepository.UpdateAsync(approval);

        return MapToDto(approval);
    }

    private MailApprovalDto MapToDto(MailApproval approval)
    {
        return new MailApprovalDto
        {
            Id = approval.Id,
            BusinessType = approval.BusinessType,
            BusinessId = approval.BusinessId,
            ApplicantId = approval.ApplicantId,
            Status = approval.Status,
            StatusName = approval.Status.ToString(),
            CurrentApproverId = approval.CurrentApproverId,
            SubmittedAt = approval.SubmittedAt,
            CompletedAt = approval.CompletedAt
        };
    }
}
