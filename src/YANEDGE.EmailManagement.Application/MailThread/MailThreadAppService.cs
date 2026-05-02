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
    private readonly IThreadAssignmentRepository _threadAssignmentRepository;
    private readonly ICurrentUser _currentUser;

    public MailThreadAppService(
        IMailThreadRepository mailThreadRepository,
        IThreadAssignmentRepository threadAssignmentRepository,
        ICurrentUser currentUser)
    {
        _mailThreadRepository = mailThreadRepository;
        _threadAssignmentRepository = threadAssignmentRepository;
        _currentUser = currentUser;
    }

    public async Task<List<MailThreadDto>> GetListAsync(GetThreadListInput input)
    {
        // Implement filtering and pagination
        var query = await _mailThreadRepository.GetQueryableAsync();

        // Apply filters if provided
        if (input.MailAccountId.HasValue)
        {
            query = query.Where(t => t.MailAccountId == input.MailAccountId.Value);
        }

        if (input.Status.HasValue)
        {
            query = query.Where(t => t.Status == input.Status.Value);
        }

        if (input.CurrentAssigneeId.HasValue)
        {
            query = query.Where(t => t.CurrentAssigneeId == input.CurrentAssigneeId.Value);
        }

        // Apply pagination
        var skipCount = input.SkipCount ?? 0;
        var maxResultCount = input.MaxResultCount ?? 20;

        var threads = query
            .OrderByDescending(t => t.LatestMessageTime)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToList();

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

        // Create ThreadAssignment record
        var assignment = new ThreadAssignment(
            GuidGenerator.Create(),
            id,
            AssigneeType.User,
            userId,
            "Claim",
            userId,
            null,
            null,
            input.Note
        );

        await _threadAssignmentRepository.InsertAsync(assignment);

        return MapToDto(thread);
    }

    public async Task<MailThreadDto> AssignAsync(Guid id, AssignThreadInput input)
    {
        var thread = await _mailThreadRepository.GetAsync(id);

        var userId = _currentUser.Id ?? throw new InvalidOperationException("User not authenticated");

        thread.Assign(input.ToAssigneeType, input.ToAssigneeId);

        await _mailThreadRepository.UpdateAsync(thread);

        // Create ThreadAssignment record
        var assignment = new ThreadAssignment(
            GuidGenerator.Create(),
            id,
            input.ToAssigneeType,
            input.ToAssigneeId,
            "Assign",
            userId,
            thread.CurrentAssigneeType,
            thread.CurrentAssigneeId,
            input.Note
        );

        await _threadAssignmentRepository.InsertAsync(assignment);

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
            UnreadCount = 0 // Will be calculated from message read status in future enhancement
        };
    }
}
