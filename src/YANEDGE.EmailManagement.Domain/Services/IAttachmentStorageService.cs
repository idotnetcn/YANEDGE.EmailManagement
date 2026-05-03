using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace YANEDGE.EmailManagement.Services;

/// <summary>
/// 附件存储服务接口
/// </summary>
public interface IAttachmentStorageService
{
    /// <summary>
    /// 保存附件
    /// </summary>
    Task<string> SaveAsync(
        string fileName,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取附件
    /// </summary>
    Task<Stream> ReadAsync(
        string storageKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除附件
    /// </summary>
    Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查附件是否存在
    /// </summary>
    Task<bool> ExistsAsync(
        string storageKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取附件URL（如果支持）
    /// </summary>
    Task<string> GetUrlAsync(
        string storageKey,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default);
}
