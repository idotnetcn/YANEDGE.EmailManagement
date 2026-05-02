namespace YANEDGE.EmailManagement.Domain.Services;

/// <summary>
/// 邮箱连接测试服务接口
/// </summary>
public interface IMailConnectionTestService
{
    /// <summary>
    /// 测试邮箱连接
    /// </summary>
    Task<MailConnectionTestResult> TestConnectionAsync(MailAccount.MailAccount account);
}

/// <summary>
/// 邮箱连接测试结果
/// </summary>
public class MailConnectionTestResult
{
    /// <summary>
    /// 接收服务器连接成功
    /// </summary>
    public bool IncomingSuccess { get; set; }

    /// <summary>
    /// 接收服务器错误信息
    /// </summary>
    public string? IncomingError { get; set; }

    /// <summary>
    /// 发送服务器连接成功
    /// </summary>
    public bool OutgoingSuccess { get; set; }

    /// <summary>
    /// 发送服务器错误信息
    /// </summary>
    public string? OutgoingError { get; set; }

    /// <summary>
    /// 详细信息
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    /// 连接是否完全成功
    /// </summary>
    public bool IsSuccess => IncomingSuccess && OutgoingSuccess;
}
