using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace YANEDGE.EmailManagement.Services.Implementation;

/// <summary>
/// 本地文件系统附件存储服务
/// </summary>
public class LocalFileAttachmentStorageService : IAttachmentStorageService, ITransientDependency
{
    private readonly string _basePath;

    public LocalFileAttachmentStorageService()
    {
        // 在生产环境中，这个路径应该从配置中读取
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Attachments");

        // 确保目录存在
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<string> SaveAsync(
        string fileName,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        // 生成唯一的存储键
        var storageKey = $"{Guid.NewGuid()}/{fileName}";
        var filePath = GetFilePath(storageKey);

        // 确保目录存在
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 保存文件
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await content.CopyToAsync(fileStream, cancellationToken);

        return storageKey;
    }

    public async Task<Stream> ReadAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(storageKey);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Attachment not found: {storageKey}");
        }

        var memoryStream = new MemoryStream();
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        await fileStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(storageKey);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);

            // 如果目录为空，也删除目录
            var directory = Path.GetDirectoryName(filePath);
            if (Directory.Exists(directory) && Directory.GetFiles(directory).Length == 0)
            {
                Directory.Delete(directory);
            }
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(storageKey);
        return Task.FromResult(File.Exists(filePath));
    }

    public Task<string> GetUrlAsync(
        string storageKey,
        TimeSpan? expiresIn = null,
        CancellationToken cancellationToken = default)
    {
        // 本地文件存储不支持直接URL访问
        // 返回API端点路径
        return Task.FromResult($"/api/mail-management/v1/attachments/download?key={Uri.EscapeDataString(storageKey)}");
    }

    private string GetFilePath(string storageKey)
    {
        // 防止路径遍历攻击
        var sanitizedKey = storageKey.Replace("..", string.Empty);
        return Path.Combine(_basePath, sanitizedKey);
    }
}
