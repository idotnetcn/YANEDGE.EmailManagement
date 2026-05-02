using Volo.Abp.Application.Services;
using Volo.Abp.Users;
using YANEDGE.EmailManagement.Application.Contracts.MailThread;
using YANEDGE.EmailManagement.Domain.MailThread;
using YANEDGE.EmailManagement.Domain.Collaboration;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Application.MailThread;

/// <summary>
/// 邮件线程应用服务
/// </summary>
public class MailThreadAppService : ApplicationService, IMailThreadAppService
{
    private readonly IMailThreadRepository _mailThreadRepository;
    private readonly ICurrentUser _currentUser;

    public MailThreadAppService(
        IMailThreadRepository mailThreadRepository,
        ICurrentUser currentUser)
    {
        _mailThreadRepository = mailThreadRepository;
        _currentUser = currentUser;
    }

    public async Task<List<MailThreadDto>> GetListAsync(GetThreadListInput input)
    {
        // TODO: Implement filtering and pagination
        var threads = await _mailThreadRepository.GetListAsync();

        return threads.Select(MapToDto).ToList();
    }

    public async Task<MailThreadDto> GetAsync(Guid id)
    {
        var thread = await _mailThreadRepository.GetAsync(id);

        return MapToDto(thread);
    }

    public async Task<MailThreadDto> ClaimAsync(Guid id, ClaimThreadInput input)
    {
        var thread = await _mailThreadRepository.GetAsync(id);

        var userId = _currentUser.Id ?? throw new InvalidOperationException("User not authenticated");

        thread.Claim(userId);

        await _mailThreadRepository.UpdateAsync(thread);

        // TODO: Create ThreadAssignment record

        return MapToDto(thread);
    }

    public async Task<MailThreadDto> AssignAsync(Guid id, AssignThreadInput input)
    {
        var thread = await _mailThreadRepository.GetAsync(id);

        thread.Assign(input.ToAssigneeType, input.ToAssigneeId);

        await _mailThreadRepository.UpdateAsync(thread);

        // TODO: Create ThreadAssignment record

        return MapToDto(thread);
    }

    public async Task<MailThreadDto> ArchiveAsync(Guid id, ArchiveThreadInput input)
    {
        var thread = await _mailThreadRepository.GetAsync(id);

        thread.Archive();

        await _mailThreadRepository.UpdateAsync(thread);

        return MapToDto(thread);
    }

    public async Task<MailThreadDto> CloseAsync(Guid id)
    {
        var thread = await _mailThreadRepository.GetAsync(id);

        thread.Close();

        await _mailThreadRepository.UpdateAsync(thread);

        return MapToDto(thread);
    }

    public async Task<MailThreadDto> ReopenAsync(Guid id)
    {
        var thread = await _mailThreadRepository.GetAsync(id);

        thread.Reopen();

        await _mailThreadRepository.UpdateAsync(thread);

        return MapToDto(thread);
    }

    private MailThreadDto MapToDto(Domain.MailThread.MailThread thread)
    {
        return new MailThreadDto
        {
            Id = thread.Id,
            MailAccountId = thread.MailAccountId,
            Subject = thread.Subject,
            Status = thread.Status,
            StatusName = thread.Status.ToString(),
            CurrentAssigneeType = thread.CurrentAssigneeType,
            CurrentAssigneeId = thread.CurrentAssigneeId,
            LatestMessageTime = thread.LatestMessageTime,
            MessageCount = thread.MessageCount,
            HasAttachment = thread.HasAttachment,
            Priority = thread.Priority,
            UnreadCount = 0 // TODO: Calculate from message read status
        };
    }
}
