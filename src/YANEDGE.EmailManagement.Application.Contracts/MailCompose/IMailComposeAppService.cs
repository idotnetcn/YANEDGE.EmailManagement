using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.Application.Contracts.MailCompose;

/// <summary>
/// 发件应用服务接口
/// </summary>
public interface IMailComposeAppService : IApplicationService
{
    /// <summary>
    /// 创建发件任务(草稿)
    /// </summary>
    Task<MailSendTaskDto> CreateAsync(CreateSendTaskInput input);

    /// <summary>
    /// 获取发件任务详情
    /// </summary>
    Task<MailSendTaskDto> GetAsync(Guid id);

    /// <summary>
    /// 提交审批
    /// </summary>
    Task<MailSendTaskDto> SubmitApprovalAsync(Guid id);

    /// <summary>
    /// 立即发送
    /// </summary>
    Task<MailSendTaskDto> SendAsync(Guid id);

    /// <summary>
    /// 取消发送任务
    /// </summary>
    Task<MailSendTaskDto> CancelAsync(Guid id);

    /// <summary>
    /// 重试发送
    /// </summary>
    Task<MailSendTaskDto> RetryAsync(Guid id);

    /// <summary>
    /// 查询发件任务列表
    /// </summary>
    Task<List<MailSendTaskDto>> GetListAsync(GetSendTaskListInput input);
}

public class GetSendTaskListInput
{
    public Guid? MailAccountId { get; set; }
    public int? Status { get; set; }
    public bool? NeedApproval { get; set; }
    public int PageNo { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
