using Volo.Abp.Application.Services;
using Volo.Abp.Users;
using YANEDGE.EmailManagement.Application.Contracts.MailCompose;
using YANEDGE.EmailManagement.Domain.MailCompose;

namespace YANEDGE.EmailManagement.Application.MailCompose;

/// <summary>
/// 发件应用服务
/// </summary>
public class MailComposeAppService : ApplicationService, IMailComposeAppService
{
    private readonly IMailSendTaskRepository _mailSendTaskRepository;
    private readonly ICurrentUser _currentUser;

    public MailComposeAppService(
        IMailSendTaskRepository mailSendTaskRepository,
        ICurrentUser currentUser)
    {
        _mailSendTaskRepository = mailSendTaskRepository;
        _currentUser = currentUser;
    }

    public async Task<MailSendTaskDto> CreateAsync(CreateSendTaskInput input)
    {
        var userId = _currentUser.Id ?? throw new InvalidOperationException("User not authenticated");

        // TODO: Check if approval is needed based on policy
        var needApproval = false;

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

        // TODO: Save recipients and attachments

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

        // TODO: Create approval and get approval ID
        var approvalId = Guid.NewGuid();

        sendTask.SubmitForApproval(approvalId);

        await _mailSendTaskRepository.UpdateAsync(sendTask);

        return MapToDto(sendTask);
    }

    public async Task<MailSendTaskDto> SendAsync(Guid id)
    {
        var sendTask = await _mailSendTaskRepository.GetAsync(id);

        // TODO: Check if approval is needed
        // If approved or no approval needed, queue for sending
        sendTask.StartSending();

        await _mailSendTaskRepository.UpdateAsync(sendTask);

        // TODO: Queue background job to send email

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

        // TODO: Queue background job to send email

        return MapToDto(sendTask);
    }

    public async Task<List<MailSendTaskDto>> GetListAsync(GetSendTaskListInput input)
    {
        // TODO: Implement filtering and pagination
        var tasks = await _mailSendTaskRepository.GetListAsync();

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
