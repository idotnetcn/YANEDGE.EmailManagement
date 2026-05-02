using Volo.Abp.Application.Services;
using Volo.Abp.Users;
using YANEDGE.EmailManagement.Application.Contracts.MailCompose;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Domain.Services;
using YANEDGE.EmailManagement.Domain.Approval;

namespace YANEDGE.EmailManagement.Application.MailCompose;

/// <summary>
/// 发件应用服务
/// </summary>
public class MailComposeAppService : ApplicationService, IMailComposeAppService
{
    private readonly IMailSendTaskRepository _mailSendTaskRepository;
    private readonly IMailApprovalRepository _mailApprovalRepository;
    private readonly IMailSendService _mailSendService;
    private readonly ICurrentUser _currentUser;

    public MailComposeAppService(
        IMailSendTaskRepository mailSendTaskRepository,
        IMailApprovalRepository mailApprovalRepository,
        IMailSendService mailSendService,
        ICurrentUser currentUser)
    {
        _mailSendTaskRepository = mailSendTaskRepository;
        _mailApprovalRepository = mailApprovalRepository;
        _mailSendService = mailSendService;
        _currentUser = currentUser;
    }

    public async Task<MailSendTaskDto> CreateAsync(CreateSendTaskInput input)
    {
        var userId = _currentUser.Id ?? throw new InvalidOperationException("User not authenticated");

        // Check if approval is needed based on policy
        // For MVP: require approval if marked as sensitive or has large attachments
        var needApproval = input.RequireApproval || (input.AttachmentCount > 10);

        var sendTask = new MailSendTask(
            GuidGenerator.Create(),
            input.MailAccountId,
            input.Subject,
            userId,
            needApproval
        );

        sendTask.SetBody(input.BodyHtml, input.BodyText);

        if (input.TemplateId.HasValue)
        {
            sendTask.SetTemplate(input.TemplateId.Value);
        }

        if (input.SignatureId.HasValue)
        {
            sendTask.SetSignature(input.SignatureId.Value);
        }

        if (input.ThreadId.HasValue)
        {
            sendTask.SetThread(input.ThreadId.Value);
        }

        if (!string.IsNullOrEmpty(input.ExternalBizRef))
        {
            sendTask.SetExternalBizRef(input.ExternalBizRef);
        }

        if (input.ScheduledSendTime.HasValue)
        {
            sendTask.SetScheduledSendTime(input.ScheduledSendTime.Value);
        }

        await _mailSendTaskRepository.InsertAsync(sendTask);

        // Recipients and attachments will be saved via separate detail endpoints or batch operations
        // This keeps the transaction scope manageable

        return MapToDto(sendTask);
    }

    public async Task<MailSendTaskDto> GetAsync(Guid id)
    {
        var sendTask = await _mailSendTaskRepository.GetAsync(id);

        return MapToDto(sendTask);
    }

    public async Task<MailSendTaskDto> SubmitApprovalAsync(Guid id)
    {
        var sendTask = await _mailSendTaskRepository.GetAsync(id);

        var userId = _currentUser.Id ?? throw new InvalidOperationException("User not authenticated");

        // Create approval record
        var approval = new MailApproval(
            GuidGenerator.Create(),
            "SendTask",
            id,
            userId,
            userId // For MVP, current user is also the first approver
        );

        await _mailApprovalRepository.InsertAsync(approval);

        sendTask.SubmitForApproval(approval.Id);

        await _mailSendTaskRepository.UpdateAsync(sendTask);

        return MapToDto(sendTask);
    }

    public async Task<MailSendTaskDto> SendAsync(Guid id)
    {
        var sendTask = await _mailSendTaskRepository.GetAsync(id);

        // Check if approval is needed and if approved
        if (sendTask.NeedApproval && sendTask.ApprovalId.HasValue)
        {
            var approval = await _mailApprovalRepository.GetAsync(sendTask.ApprovalId.Value);
            if (approval.Status != Enums.ApprovalStatus.Approved)
            {
                throw new Volo.Abp.BusinessException("SendTask:NotApproved")
                    .WithData("SendTaskId", id);
            }
        }

        sendTask.StartSending();

        await _mailSendTaskRepository.UpdateAsync(sendTask);

        // Queue background job to send email
        await _mailSendService.QueueSendTaskAsync(sendTask);

        return MapToDto(sendTask);
    }

    public async Task<MailSendTaskDto> CancelAsync(Guid id)
    {
        var sendTask = await _mailSendTaskRepository.GetAsync(id);

        sendTask.Cancel();

        await _mailSendTaskRepository.UpdateAsync(sendTask);

        return MapToDto(sendTask);
    }

    public async Task<MailSendTaskDto> RetryAsync(Guid id)
    {
        var sendTask = await _mailSendTaskRepository.GetAsync(id);

        sendTask.Retry();

        await _mailSendTaskRepository.UpdateAsync(sendTask);

        // Queue background job to send email
        await _mailSendService.QueueSendTaskAsync(sendTask);

        return MapToDto(sendTask);
    }

    public async Task<List<MailSendTaskDto>> GetListAsync(GetSendTaskListInput input)
    {
        // Implement filtering and pagination
        var query = await _mailSendTaskRepository.GetQueryableAsync();

        // Apply filters if provided
        if (input.MailAccountId.HasValue)
        {
            query = query.Where(t => t.MailAccountId == input.MailAccountId.Value);
        }

        if (input.Status.HasValue)
        {
            query = query.Where(t => t.Status == input.Status.Value);
        }

        if (input.CreatedByUserId.HasValue)
        {
            query = query.Where(t => t.CreatedByUserId == input.CreatedByUserId.Value);
        }

        // Apply pagination
        var skipCount = input.SkipCount ?? 0;
        var maxResultCount = input.MaxResultCount ?? 20;

        var tasks = query
            .OrderByDescending(t => t.CreatedAt)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToList();

        return tasks.Select(MapToDto).ToList();
    }

    private MailSendTaskDto MapToDto(MailSendTask sendTask)
    {
        return new MailSendTaskDto
        {
            Id = sendTask.Id,
            MailAccountId = sendTask.MailAccountId,
            ThreadId = sendTask.ThreadId,
            Subject = sendTask.Subject,
            Status = sendTask.Status,
            StatusName = sendTask.Status.ToString(),
            NeedApproval = sendTask.NeedApproval,
            ApprovalId = sendTask.ApprovalId,
            ScheduledSendTime = sendTask.ScheduledSendTime,
            ActualSentTime = sendTask.ActualSentTime,
            RetryCount = sendTask.RetryCount,
            ErrorCode = sendTask.ErrorCode,
            ErrorMessage = sendTask.ErrorMessage,
            CreatedByUserId = sendTask.CreatedByUserId,
            CreatedAt = sendTask.CreatedAt
        };
    }
}
