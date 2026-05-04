using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Application.Contracts.Search;

/// <summary>
/// 邮件搜索应用服务接口
/// </summary>
public interface IMailSearchAppService
{
    /// <summary>
    /// 搜索邮件
    /// </summary>
    Task<PagedResultDto<MailSearchResultDto>> SearchAsync(MailSearchInput input);

    /// <summary>
    /// 检查搜索服务健康状态
    /// </summary>
    Task<SearchHealthDto> GetHealthAsync();

    /// <summary>
    /// 手动触发索引同步
    /// </summary>
    Task TriggerIndexSyncAsync(IndexSyncType syncType);
}

/// <summary>
/// 邮件搜索输入
/// </summary>
public class MailSearchInput : PagedAndSortedResultRequestDto
{
    public string Query { get; set; } = string.Empty;
    public Guid? MailAccountId { get; set; }
    public Guid? ThreadId { get; set; }
    public string? FromAddress { get; set; }
    public string? ToAddress { get; set; }
    public bool? HasAttachments { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string>? Labels { get; set; }
}

/// <summary>
/// 邮件搜索结果DTO
/// </summary>
public class MailSearchResultDto
{
    public Guid Id { get; set; }
    public Guid ThreadId { get; set; }
    public Guid MailAccountId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public List<string> ToAddresses { get; set; } = new();
    public string BodyPreview { get; set; } = string.Empty;
    public bool HasAttachments { get; set; }
    public List<string> AttachmentNames { get; set; } = new();
    public List<string> Labels { get; set; } = new();
    public DateTime ReceivedTime { get; set; }
    public float Score { get; set; }
}

/// <summary>
/// 搜索服务健康状态DTO
/// </summary>
public class SearchHealthDto
{
    public bool IsAvailable { get; set; }
    public bool IsEnabled { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// 索引同步类型
/// </summary>
public enum IndexSyncType
{
    /// <summary>
    /// 全量同步
    /// </summary>
    Full = 0,

    /// <summary>
    /// 增量同步（最近24小时）
    /// </summary>
    Incremental = 1
}
