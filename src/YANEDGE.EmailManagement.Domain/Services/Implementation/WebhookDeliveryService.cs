using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using YANEDGE.EmailManagement.Domain.Webhook;

namespace YANEDGE.EmailManagement.Services.Implementation;

/// <summary>
/// Webhook投递服务
/// Service for delivering webhook notifications to external systems
/// </summary>
public class WebhookDeliveryService : DomainService, ITransientDependency
{
    private readonly IWebhookDeliveryLogRepository _deliveryLogRepository;
    private readonly IHttpClientFactory _httpClientFactory;

    public WebhookDeliveryService(
        IWebhookDeliveryLogRepository deliveryLogRepository,
        IHttpClientFactory httpClientFactory)
    {
        _deliveryLogRepository = deliveryLogRepository;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// 创建投递日志
    /// </summary>
    public async Task<WebhookDeliveryLog> CreateDeliveryLogAsync(
        WebhookSubscription subscription,
        Guid eventId,
        string eventType,
        object eventData)
    {
        var payload = JsonSerializer.Serialize(new
        {
            eventId = eventId,
            eventType = eventType,
            eventTime = DateTime.UtcNow,
            data = eventData
        });

        var log = new WebhookDeliveryLog(
            GuidGenerator.Create(),
            subscription.Id,
            eventId,
            eventType,
            payload,
            subscription.Url,
            subscription.TenantId
        );

        await _deliveryLogRepository.InsertAsync(log, autoSave: true);
        return log;
    }

    /// <summary>
    /// 执行Webhook投递
    /// </summary>
    public async Task<bool> DeliverWebhookAsync(
        WebhookDeliveryLog log,
        WebhookSubscription subscription)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(subscription.TimeoutSeconds);

            var request = new HttpRequestMessage(HttpMethod.Post, subscription.Url);

            // 添加内容
            request.Content = new StringContent(log.Payload, Encoding.UTF8, "application/json");

            // 生成签名
            var signature = GenerateSignature(log.Payload, subscription.Secret);
            request.Headers.Add("X-EmailManagement-Signature", signature);
            request.Headers.Add("X-EmailManagement-Event-Type", log.EventType);
            request.Headers.Add("X-EmailManagement-Event-Id", log.EventId.ToString());

            // 添加自定义头
            if (subscription.Headers != null)
            {
                foreach (var header in subscription.Headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // 发送请求
            var response = await httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                log.MarkAsSuccessful((int)response.StatusCode, responseBody);
                await _deliveryLogRepository.UpdateAsync(log, autoSave: true);
                Logger.LogInformation(
                    "Webhook delivered successfully. EventType: {EventType}, URL: {Url}",
                    log.EventType, subscription.Url);
                return true;
            }
            else
            {
                var errorMessage = $"HTTP {response.StatusCode}: {response.ReasonPhrase}";
                var nextRetryTime = CalculateNextRetryTime(log.RetryCount + 1, subscription.MaxRetryCount);

                log.MarkAsFailed(
                    (int)response.StatusCode,
                    responseBody,
                    errorMessage,
                    log.RetryCount + 1,
                    nextRetryTime
                );

                await _deliveryLogRepository.UpdateAsync(log, autoSave: true);

                Logger.LogWarning(
                    "Webhook delivery failed. EventType: {EventType}, URL: {Url}, StatusCode: {StatusCode}",
                    log.EventType, subscription.Url, response.StatusCode);

                return false;
            }
        }
        catch (Exception ex)
        {
            var nextRetryTime = CalculateNextRetryTime(log.RetryCount + 1, subscription.MaxRetryCount);

            log.MarkAsFailed(
                ex.Message,
                log.RetryCount + 1,
                nextRetryTime
            );

            await _deliveryLogRepository.UpdateAsync(log, autoSave: true);

            Logger.LogError(ex,
                "Webhook delivery exception. EventType: {EventType}, URL: {Url}",
                log.EventType, subscription.Url);

            return false;
        }
    }

    /// <summary>
    /// 生成HMAC-SHA256签名
    /// </summary>
    private string GenerateSignature(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// 计算下次重试时间(指数退避)
    /// </summary>
    private DateTime? CalculateNextRetryTime(int retryCount, int maxRetryCount)
    {
        if (retryCount > maxRetryCount)
        {
            return null; // 超过最大重试次数，不再重试
        }

        // 指数退避: 60秒, 5分钟, 15分钟, 1小时, 2小时
        var delays = new[] { 60, 300, 900, 3600, 7200 };
        var delaySeconds = retryCount <= delays.Length
            ? delays[retryCount - 1]
            : delays[^1];

        return DateTime.UtcNow.AddSeconds(delaySeconds);
    }
}
