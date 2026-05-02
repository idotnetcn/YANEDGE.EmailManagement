using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.Application.Contracts.MailAccount;

/// <summary>
/// 邮箱账号应用服务接口
/// </summary>
public interface IMailAccountAppService : IApplicationService
{
    /// <summary>
    /// 获取邮箱账号列表
    /// </summary>
    Task<List<MailAccountDto>> GetListAsync();

    /// <summary>
    /// 获取邮箱账号详情
    /// </summary>
    Task<MailAccountDto> GetAsync(Guid id);

    /// <summary>
    /// 创建邮箱账号
    /// </summary>
    Task<MailAccountDto> CreateAsync(CreateMailAccountInput input);

    /// <summary>
    /// 启用/停用同步
    /// </summary>
    Task<MailAccountDto> ToggleSyncAsync(Guid id, bool enabled);

    /// <summary>
    /// 测试邮箱连接
    /// </summary>
    Task<TestConnectionResult> TestConnectionAsync(Guid id);

    /// <summary>
    /// 手动触发邮箱同步
    /// </summary>
    Task<SyncJobResult> TriggerSyncAsync(Guid id);
}

public class TestConnectionResult
{
    public bool IncomingSuccess { get; set; }
    public bool OutgoingSuccess { get; set; }
    public string Detail { get; set; } = string.Empty;
}

public class SyncJobResult
{
    public Guid JobId { get; set; }
    public bool Accepted { get; set; }
    public string Message { get; set; } = string.Empty;
}
