using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace YANEDGE.EmailManagement.Domain.Attachment;

/// <summary>
/// 邮件附件聚合根
/// </summary>
public class MailAttachment : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 所属邮件ID
    /// </summary>
    public Guid MailMessageId { get; private set; }

    /// <summary>
    /// 附件原始文件名
    /// </summary>
    public string FileName { get; private set; }

    /// <summary>
    /// 内容类型 (MIME Type)
    /// </summary>
    public string ContentType { get; private set; }

    /// <summary>
    /// 文件大小 (字节)
    /// </summary>
    public long FileSize { get; private set; }

    /// <summary>
    /// 存储路径
    /// </summary>
    public string StoragePath { get; private set; }

    /// <summary>
    /// 文件哈希值 (SHA256)
    /// </summary>
    public string? FileHash { get; private set; }

    /// <summary>
    /// 内容ID (用于内嵌图片引用)
    /// </summary>
    public string? ContentId { get; private set; }

    /// <summary>
    /// 是否内联附件
    /// </summary>
    public bool IsInline { get; private set; }

    /// <summary>
    /// 是否敏感附件
    /// </summary>
    public bool IsSensitive { get; private set; }

    /// <summary>
    /// 下载次数
    /// </summary>
    public int DownloadCount { get; private set; }

    /// <summary>
    /// 最后下载时间
    /// </summary>
    public DateTime? LastDownloadedAt { get; private set; }

    /// <summary>
    /// 是否已病毒扫描
    /// </summary>
    public bool IsScanned { get; private set; }

    /// <summary>
    /// 是否安全
    /// </summary>
    public bool IsSafe { get; private set; }

    /// <summary>
    /// 扫描结果信息
    /// </summary>
    public string? ScanResult { get; private set; }

    protected MailAttachment()
    {
        FileName = string.Empty;
        ContentType = string.Empty;
        StoragePath = string.Empty;
    }

    public MailAttachment(
        Guid id,
        Guid mailMessageId,
        string fileName,
        string contentType,
        long fileSize,
        string storagePath,
        string? fileHash = null,
        string? contentId = null,
        bool isInline = false,
        bool isSensitive = false
    ) : base(id)
    {
        MailMessageId = mailMessageId;
        FileName = fileName;
        ContentType = contentType;
        FileSize = fileSize;
        StoragePath = storagePath;
        FileHash = fileHash;
        ContentId = contentId;
        IsInline = isInline;
        IsSensitive = isSensitive;
        DownloadCount = 0;
        IsScanned = false;
        IsSafe = false;
    }

    public void RecordDownload()
    {
        DownloadCount++;
        LastDownloadedAt = DateTime.UtcNow;
    }

    public void MarkAsSensitive()
    {
        IsSensitive = true;
    }

    public void UpdateScanResult(bool isSafe, string? scanResult = null)
    {
        IsScanned = true;
        IsSafe = isSafe;
        ScanResult = scanResult;
    }
}
