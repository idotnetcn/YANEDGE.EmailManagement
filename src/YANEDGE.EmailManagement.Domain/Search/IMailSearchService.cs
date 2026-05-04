using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace YANEDGE.EmailManagement.Domain.Search;

/// <summary>
/// 邮件搜索服务接口
/// Elasticsearch abstraction for mail search
/// </summary>
public interface IMailSearchService
{
    /// <summary>
    /// 索引邮件消息
    /// </summary>
    Task IndexMailMessageAsync(Guid messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量索引邮件消息
    /// </summary>
    Task BulkIndexMailMessagesAsync(IEnumerable<Guid> messageIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除邮件索引
    /// </summary>
    Task DeleteMailMessageIndexAsync(Guid messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索邮件
    /// </summary>
    Task<MailSearchResult> SearchAsync(MailSearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查Elasticsearch连接状态
    /// </summary>
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建索引（如果不存在）
    /// </summary>
    Task EnsureIndexAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 邮件搜索请求
/// </summary>
public class MailSearchRequest
{
    /// <summary>
    /// 搜索关键词
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// 邮件账户ID过滤
    /// </summary>
    public Guid? MailAccountId { get; set; }

    /// <summary>
    /// 会话ID过滤
    /// </summary>
    public Guid? ThreadId { get; set; }

    /// <summary>
    /// 发件人过滤
    /// </summary>
    public string? FromAddress { get; set; }

    /// <summary>
    /// 收件人过滤
    /// </summary>
    public string? ToAddress { get; set; }

    /// <summary>
    /// 是否有附件
    /// </summary>
    public bool? HasAttachments { get; set; }

    /// <summary>
    /// 日期范围-开始
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 日期范围-结束
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 标签过滤
    /// </summary>
    public List<string>? Labels { get; set; }

    /// <summary>
    /// 跳过记录数
    /// </summary>
    public int Skip { get; set; } = 0;

    /// <summary>
    /// 获取记录数
    /// </summary>
    public int Take { get; set; } = 20;

    /// <summary>
    /// 排序字段
    /// </summary>
    public string? SortField { get; set; }

    /// <summary>
    /// 排序方向 (asc/desc)
    /// </summary>
    public string? SortDirection { get; set; }
}

/// <summary>
/// 邮件搜索结果
/// </summary>
public class MailSearchResult
{
    /// <summary>
    /// 总记录数
    /// </summary>
    public long TotalCount { get; set; }

    /// <summary>
    /// 搜索结果列表
    /// </summary>
    public List<MailSearchDocument> Items { get; set; } = new();

    /// <summary>
    /// 搜索耗时（毫秒）
    /// </summary>
    public long ElapsedMilliseconds { get; set; }
}

/// <summary>
/// 邮件搜索文档
/// </summary>
public class MailSearchDocument
{
    public Guid Id { get; set; }
    public Guid ThreadId { get; set; }
    public Guid MailAccountId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public List<string> ToAddresses { get; set; } = new();
    public List<string> CcAddresses { get; set; } = new();
    public string BodyPreview { get; set; } = string.Empty;
    public string BodyText { get; set; } = string.Empty;
    public bool HasAttachments { get; set; }
    public List<string> AttachmentNames { get; set; } = new();
    public List<string> Labels { get; set; } = new();
    public DateTime ReceivedTime { get; set; }
    public DateTime IndexedAt { get; set; }
    public float Score { get; set; }
}
