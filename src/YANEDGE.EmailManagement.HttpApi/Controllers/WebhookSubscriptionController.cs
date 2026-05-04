using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using YANEDGE.EmailManagement.Webhook;

namespace YANEDGE.EmailManagement.HttpApi.Controllers;

/// <summary>
/// Webhook订阅管理控制器
/// </summary>
[Route("api/mail-management/v1/webhooks")]
public class WebhookSubscriptionController : AbpControllerBase
{
    private readonly IWebhookSubscriptionAppService _webhookSubscriptionAppService;

    public WebhookSubscriptionController(IWebhookSubscriptionAppService webhookSubscriptionAppService)
    {
        _webhookSubscriptionAppService = webhookSubscriptionAppService;
    }

    /// <summary>
    /// 获取Webhook订阅列表
    /// </summary>
    [HttpGet]
    public Task<PagedResultDto<WebhookSubscriptionDto>> GetListAsync([FromQuery] WebhookSubscriptionGetListInput input)
    {
        return _webhookSubscriptionAppService.GetListAsync(input);
    }

    /// <summary>
    /// 获取指定Webhook订阅
    /// </summary>
    [HttpGet("{id}")]
    public Task<WebhookSubscriptionDto> GetAsync(Guid id)
    {
        return _webhookSubscriptionAppService.GetAsync(id);
    }

    /// <summary>
    /// 创建Webhook订阅
    /// </summary>
    [HttpPost]
    public Task<WebhookSubscriptionDto> CreateAsync([FromBody] CreateWebhookSubscriptionDto input)
    {
        return _webhookSubscriptionAppService.CreateAsync(input);
    }

    /// <summary>
    /// 更新Webhook订阅
    /// </summary>
    [HttpPut("{id}")]
    public Task<WebhookSubscriptionDto> UpdateAsync(Guid id, [FromBody] UpdateWebhookSubscriptionDto input)
    {
        return _webhookSubscriptionAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// 删除Webhook订阅
    /// </summary>
    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _webhookSubscriptionAppService.DeleteAsync(id);
    }

    /// <summary>
    /// 激活Webhook订阅
    /// </summary>
    [HttpPost("{id}/activate")]
    public Task<WebhookSubscriptionDto> ActivateAsync(Guid id)
    {
        return _webhookSubscriptionAppService.ActivateAsync(id);
    }

    /// <summary>
    /// 停用Webhook订阅
    /// </summary>
    [HttpPost("{id}/deactivate")]
    public Task<WebhookSubscriptionDto> DeactivateAsync(Guid id)
    {
        return _webhookSubscriptionAppService.DeactivateAsync(id);
    }

    /// <summary>
    /// 获取可用的事件类型列表
    /// </summary>
    [HttpGet("available-event-types")]
    public Task<List<string>> GetAvailableEventTypesAsync()
    {
        return _webhookSubscriptionAppService.GetAvailableEventTypesAsync();
    }
}
