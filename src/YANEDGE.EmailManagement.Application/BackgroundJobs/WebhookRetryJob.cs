using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using YANEDGE.EmailManagement.Domain.Webhook;
using YANEDGE.EmailManagement.Services.Implementation;

namespace YANEDGE.EmailManagement.Application.BackgroundJobs;

/// <summary>
/// Webhook重试后台任务
/// Processes failed webhook deliveries for retry
/// </summary>
public class WebhookRetryJob : ITransientDependency
{
    private readonly IWebhookDeliveryLogRepository _deliveryLogRepository;
    private readonly IWebhookSubscriptionRepository _subscriptionRepository;
    private readonly WebhookDeliveryService _deliveryService;
    private readonly ILogger<WebhookRetryJob> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    private const string JobName = "WebhookRetry";
    private const int MaxRetryBatchSize = 20; // 每批次最多重试20个投递

    public WebhookRetryJob(
        IWebhookDeliveryLogRepository deliveryLogRepository,
        IWebhookSubscriptionRepository subscriptionRepository,
        WebhookDeliveryService deliveryService,
        ILogger<WebhookRetryJob> logger,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _deliveryLogRepository = deliveryLogRepository;
        _subscriptionRepository = subscriptionRepository;
        _deliveryService = deliveryService;
        _logger = logger;
        _unitOfWorkManager = unitOfWorkManager;
    }

    /// <summary>
    /// 执行Webhook重试
    /// </summary>
    public async Task ExecuteAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("[{JobName}] Webhook重试开始执行...", JobName);

        var retrySuccessCount = 0;
        var retryFailCount = 0;
        var totalRetries = 0;

        try
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

            // 获取所有需要重试的失败投递
            var failedDeliveries = await _deliveryLogRepository.GetFailedDeliveriesForRetryAsync(
                DateTime.UtcNow,
                MaxRetryBatchSize);
            totalRetries = failedDeliveries.Count;

            if (totalRetries == 0)
            {
                _logger.LogInformation("[{JobName}] 没有需要重试的Webhook投递", JobName);
                await uow.CompleteAsync();
                return;
            }

            _logger.LogInformation("[{JobName}] 找到 {Count} 个需要重试的Webhook投递", JobName, totalRetries);

            foreach (var deliveryLog in failedDeliveries)
            {
                try
                {
                    _logger.LogDebug(
                        "[{JobName}] 开始重试Webhook: {DeliveryId}, EventType: {EventType}, 当前重试次数: {RetryCount}",
                        JobName,
                        deliveryLog.Id,
                        deliveryLog.EventType,
                        deliveryLog.RetryCount);

                    // 获取订阅信息
                    var subscription = await _subscriptionRepository.GetAsync(deliveryLog.SubscriptionId);

                    if (!subscription.IsActive)
                    {
                        _logger.LogWarning("[{JobName}] 订阅已停用，停止重试: {SubscriptionId}",
                            JobName, subscription.Id);

                        deliveryLog.MarkAsFailed(
                            "Subscription is inactive",
                            deliveryLog.RetryCount,
                            null // 停止重试
                        );
                        await _deliveryLogRepository.UpdateAsync(deliveryLog, autoSave: false);
                        retryFailCount++;
                        continue;
                    }

                    // 检查是否超过最大重试次数
                    if (deliveryLog.RetryCount >= subscription.MaxRetryCount)
                    {
                        _logger.LogWarning(
                            "[{JobName}] Webhook投递 {DeliveryId} 已达最大重试次数 {MaxRetry}，将不再重试",
                            JobName, deliveryLog.Id, subscription.MaxRetryCount);

                        deliveryLog.MarkAsFailed(
                            $"Maximum retry count ({subscription.MaxRetryCount}) reached",
                            deliveryLog.RetryCount,
                            null
                        );
                        await _deliveryLogRepository.UpdateAsync(deliveryLog, autoSave: false);
                        retryFailCount++;
                        continue;
                    }

                    // 执行重试
                    var success = await _deliveryService.DeliverWebhookAsync(deliveryLog, subscription);

                    if (success)
                    {
                        retrySuccessCount++;
                        _logger.LogInformation("[{JobName}] Webhook重试成功: {DeliveryId}", JobName, deliveryLog.Id);
                    }
                    else
                    {
                        retryFailCount++;
                        _logger.LogWarning(
                            "[{JobName}] Webhook重试失败: {DeliveryId}, 重试次数: {RetryCount}/{MaxRetry}",
                            JobName, deliveryLog.Id, deliveryLog.RetryCount, subscription.MaxRetryCount);
                    }
                }
                catch (OperationCanceledException)
                {
                    retryFailCount++;
                    _logger.LogWarning("[{JobName}] Webhook重试已取消: {DeliveryId}", JobName, deliveryLog.Id);
                }
                catch (Exception ex)
                {
                    retryFailCount++;
                    _logger.LogError(ex, "[{JobName}] Webhook重试异常: {DeliveryId}", JobName, deliveryLog.Id);
                    // 继续处理下一个重试，不中断整个批次
                }
            }

            await uow.CompleteAsync();

            stopwatch.Stop();
            var successRate = totalRetries > 0 ? (double)retrySuccessCount / totalRetries * 100 : 0;

            _logger.LogInformation(
                "[{JobName}] Webhook重试完成。总计: {Total}, 重试成功: {Success}, 重试失败: {Fail}, 成功率: {Rate:F2}%, 耗时: {Duration}ms",
                JobName, totalRetries, retrySuccessCount, retryFailCount, successRate, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "[{JobName}] Webhook重试出错，耗时: {Duration}ms",
                JobName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
