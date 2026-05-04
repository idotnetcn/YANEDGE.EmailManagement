using Volo.Abp.Application.Services;
using Volo.Abp.Users;
using YANEDGE.EmailManagement.Application.Contracts.MailThread;
using YANEDGE.EmailManagement.Domain.MailThread;
using YANEDGE.EmailManagement.Domain.Collaboration;
using YANEDGE.EmailManagement.Enums;
using YANEDGE.EmailManagement.Services.Cache;

namespace YANEDGE.EmailManagement.Application.MailThread;

/// <summary>
/// 邮件线程应用服务
/// </summary>
public class MailThreadAppService : ApplicationService, IMailThreadAppService
{
    private readonly IMailThreadRepository _mailThreadRepository;
    private readonly IThreadAssignmentRepository _threadAssignmentRepository;
    private readonly IThreadSummaryCacheService _threadSummaryCacheService;
    private readonly ICurrentUser _currentUser;

    public MailThreadAppService(
        IMailThreadRepository mailThreadRepository,
        IThreadAssignmentRepository threadAssignmentRepository,
        IThreadSummaryCacheService threadSummaryCacheService,
        ICurrentUser currentUser)
    {
        _mailThreadRepository = mailThreadRepository;
        _threadAssignmentRepository = threadAssignmentRepository;
        _threadSummaryCacheService = threadSummaryCacheService;
        _currentUser = currentUser;
    }

    public async Task<List<MailThreadDto>> GetListAsync(GetThreadListInput input)
    {
        // Use optimized paginated query with AsNoTracking
        var (threads, totalCount) = await _mailThreadRepository.GetPagedListAsync(
            mailAccountId: input.MailAccountId,
            status: input.Status,
            assigneeId: input.CurrentAssigneeId,
            skipCount: input.SkipCount ?? 0,
            maxResultCount: input.MaxResultCount ?? 20);

        return threads.Select(MapToDto).ToList();
    }

    public async Task<MailThreadDto> GetAsync(Guid id)
    {
        // Try to get from cache first
        var cachedSummary = await _threadSummaryCacheService.GetThreadSummaryAsync(id);
        if (cachedSummary != null)
        {
            return MapCacheToDto(cachedSummary);
        }

        // Cache miss - load from database
        var thread = await _mailThreadRepository.GetWithoutTrackingAsync(id);
        if (thread == null)
        {
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Domain.MailThread.MailThread), id);
        }

        var dto = MapToDto(thread);

        // Cache the result
        var cacheModel = MapToCache(thread);
        await _threadSummaryCacheService.SetThreadSummaryAsync(cacheModel);

        return dto;
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

        // Invalidate cache after update
        await _threadSummaryCacheService.RemoveThreadSummaryAsync(id);

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

        // Invalidate cache after update
        await _threadSummaryCacheService.RemoveThreadSummaryAsync(id);

        return MapToDto(thread);
    }

    public async Task<MailThreadDto> ArchiveAsync(Guid id, ArchiveThreadInput input)
    {
        var thread = await _mailThreadRepository.GetAsync(id);

        thread.Archive();

        await _mailThreadRepository.UpdateAsync(thread);

        // Invalidate cache after update
        await _threadSummaryCacheService.RemoveThreadSummaryAsync(id);

        return MapToDto(thread);
    }

    public async Task<MailThreadDto> CloseAsync(Guid id)
    {
        var thread = await _mailThreadRepository.GetAsync(id);

        thread.Close();

        await _mailThreadRepository.UpdateAsync(thread);

        // Invalidate cache after update
        await _threadSummaryCacheService.RemoveThreadSummaryAsync(id);

        return MapToDto(thread);
    }

    public async Task<MailThreadDto> ReopenAsync(Guid id)
    {
        var thread = await _mailThreadRepository.GetAsync(id);

        thread.Reopen();

        await _mailThreadRepository.UpdateAsync(thread);

        // Invalidate cache after update
        await _threadSummaryCacheService.RemoveThreadSummaryAsync(id);

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

    private MailThreadDto MapCacheToDto(ThreadSummaryCache cache)
    {
        return new MailThreadDto
        {
            Id = cache.ThreadId,
            MailAccountId = cache.MailAccountId,
            Subject = cache.Subject,
            Status = (ThreadStatus)cache.ThreadStatus,
            StatusName = ((ThreadStatus)cache.ThreadStatus).ToString(),
            CurrentAssigneeType = null, // Not stored in cache
            CurrentAssigneeId = cache.CurrentAssigneeId,
            LatestMessageTime = cache.LatestMessageTime,
            MessageCount = cache.MessageCount,
            HasAttachment = cache.HasAttachment,
            Priority = cache.Priority,
            UnreadCount = cache.UnreadCount
        };
    }

    private ThreadSummaryCache MapToCache(Domain.MailThread.MailThread thread)
    {
        return new ThreadSummaryCache
        {
            ThreadId = thread.Id,
            MailAccountId = thread.MailAccountId,
            Subject = thread.Subject,
            ThreadStatus = (int)thread.Status,
            LatestMessageTime = thread.LatestMessageTime,
            UnreadCount = 0,
            MessageCount = thread.MessageCount,
            HasAttachment = thread.HasAttachment,
            Priority = thread.Priority,
            CurrentAssigneeId = thread.CurrentAssigneeId,
            Version = 1,
            CachedAt = DateTime.UtcNow
        };
    }
}
