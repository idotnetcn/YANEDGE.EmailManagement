using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using YANEDGE.EmailManagement.Application.Contracts.Search;

namespace YANEDGE.EmailManagement.HttpApi.Controllers;

/// <summary>
/// 邮件搜索控制器
/// </summary>
[Route("api/mail-management/v1/search")]
public class MailSearchController : AbpControllerBase
{
    private readonly IMailSearchAppService _searchAppService;

    public MailSearchController(IMailSearchAppService searchAppService)
    {
        _searchAppService = searchAppService;
    }

    /// <summary>
    /// 搜索邮件
    /// </summary>
    [HttpGet]
    public Task<PagedResultDto<MailSearchResultDto>> SearchAsync([FromQuery] MailSearchInput input)
    {
        return _searchAppService.SearchAsync(input);
    }

    /// <summary>
    /// 获取搜索服务健康状态
    /// </summary>
    [HttpGet("health")]
    public Task<SearchHealthDto> GetHealthAsync()
    {
        return _searchAppService.GetHealthAsync();
    }

    /// <summary>
    /// 手动触发索引同步
    /// </summary>
    [HttpPost("sync")]
    public Task TriggerIndexSyncAsync([FromBody] TriggerIndexSyncRequest request)
    {
        return _searchAppService.TriggerIndexSyncAsync(request.SyncType);
    }
}

public class TriggerIndexSyncRequest
{
    public IndexSyncType SyncType { get; set; } = IndexSyncType.Incremental;
}
