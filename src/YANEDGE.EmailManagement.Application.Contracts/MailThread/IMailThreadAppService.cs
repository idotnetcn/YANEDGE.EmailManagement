using Volo.Abp.Application.Services;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Application.Contracts.MailThread;

/// <summary>
/// 邮件线程应用服务接口
/// </summary>
public interface IMailThreadAppService : IApplicationService
{
    /// <summary>
    /// 获取线程列表
    /// </summary>
    Task<List<MailThreadDto>> GetListAsync(GetThreadListInput input);

    /// <summary>
    /// 获取线程详情
    /// </summary>
    Task<MailThreadDto> GetAsync(Guid id);

    /// <summary>
    /// 认领线程
    /// </summary>
    Task<MailThreadDto> ClaimAsync(Guid id, ClaimThreadInput input);

    /// <summary>
    /// 分派线程
    /// </summary>
    Task<MailThreadDto> AssignAsync(Guid id, AssignThreadInput input);

    /// <summary>
    /// 归档线程
    /// </summary>
    Task<MailThreadDto> ArchiveAsync(Guid id, ArchiveThreadInput input);

    /// <summary>
    /// 关闭线程
    /// </summary>
    Task<MailThreadDto> CloseAsync(Guid id);

    /// <summary>
    /// 恢复线程
    /// </summary>
    Task<MailThreadDto> ReopenAsync(Guid id);
}

public class GetThreadListInput
{
    public Guid? MailAccountId { get; set; }
    public ThreadStatus? Status { get; set; }
    public Guid? AssigneeId { get; set; }
    public string? Keyword { get; set; }
    public int PageNo { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class ClaimThreadInput
{
    public string? Reason { get; set; }
}

public class AssignThreadInput
{
    public AssigneeType ToAssigneeType { get; set; }
    public Guid ToAssigneeId { get; set; }
    public string? Reason { get; set; }
}

public class ArchiveThreadInput
{
    public string? Remark { get; set; }
}
