using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using YANEDGE.EmailManagement.Domain.Webhook;
using YANEDGE.EmailManagement.Services.Implementation;

namespace YANEDGE.EmailManagement.Application.BackgroundJobs;

/// <summary>
/// Webhook投递后台任务
/// Processes pending webhook deliveries
/// </summary>
public class WebhookDeliveryJob : ITransientDependency
{
    private readonly IWebhookDeliveryLogRepository _deliveryLogRepository;
    private readonly IWebhookSubscriptionRepository _subscriptionRepository;
    private readonly WebhookDeliveryService _deliveryService;
    private readonly ILogger<WebhookDeliveryJob> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    private const string JobName = "WebhookDelivery";
    private const int MaxBatchSize = 50; // 每批次最多处理50个投递

    public WebhookDeliveryJob(
        IWebhookDeliveryLogRepository deliveryLogRepository,
        IWebhookSubscriptionRepository subscriptionRepository,
        WebhookDeliveryService deliveryService,
        ILogger<WebhookDeliveryJob> logger,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _deliveryLogRepository = deliveryLogRepository;
        _subscriptionRepository = subscriptionRepository;
        _deliveryService = deliveryService;
        _logger = logger;
        _unitOfWorkManager = unitOfWorkManager;
    }

    /// <summary>
    /// 执行Webhook投递
    /// </summary>
    public async Task ExecuteAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("[{JobName}] Webhook投递开始执行...", JobName);

        var successCount = 0;
        var failCount = 0;
        var totalDeliveries = 0;

        try
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

            // 获取所有待投递的日志
            var pendingDeliveries = await _deliveryLogRepository.GetPendingDeliveriesAsync(MaxBatchSize);
            totalDeliveries = pendingDeliveries.Count;

            if (totalDeliveries == 0)
            {
                _logger.LogInformation("[{JobName}] 没有待投递的Webhook", JobName);
                await uow.CompleteAsync();
                return;
            }

            _logger.LogInformation("[{JobName}] 找到 {Count} 个待投递的Webhook", JobName, totalDeliveries);

            foreach (var deliveryLog in pendingDeliveries)
            {
                try
                {
                    _logger.LogDebug("[{JobName}] 开始投递Webhook: {DeliveryId}, EventType: {EventType}",
                        JobName, deliveryLog.Id, deliveryLog.EventType);

                    // 获取订阅信息
                    var subscription = await _subscriptionRepository.GetAsync(deliveryLog.SubscriptionId);

                    if (!subscription.IsActive)
                    {
                        _logger.LogWarning("[{JobName}] 订阅已停用，跳过投递: {SubscriptionId}",
                            JobName, subscription.Id);

                        deliveryLog.MarkAsFailed(
                            "Subscription is inactive",
                            deliveryLog.RetryCount,
                            null
                        );
                        await _deliveryLogRepository.UpdateAsync(deliveryLog, autoSave: false);
                        failCount++;
                        continue;
                    }

                    // 执行投递
                    var success = await _deliveryService.DeliverWebhookAsync(deliveryLog, subscription);

                    if (success)
                    {
                        successCount++;
                        _logger.LogDebug("[{JobName}] Webhook投递成功: {DeliveryId}", JobName, deliveryLog.Id);
                    }
                    else
                    {
                        failCount++;
                        _logger.LogWarning("[{JobName}] Webhook投递失败: {DeliveryId}", JobName, deliveryLog.Id);
                    }
                }
                catch (OperationCanceledException)
                {
                    failCount++;
                    _logger.LogWarning("[{JobName}] Webhook投递已取消: {DeliveryId}", JobName, deliveryLog.Id);
                }
                catch (Exception ex)
                {
                    failCount++;
                    _logger.LogError(ex, "[{JobName}] Webhook投递异常: {DeliveryId}", JobName, deliveryLog.Id);
                    // 继续处理下一个投递，不中断整个批次
                }
            }

            await uow.CompleteAsync();

            stopwatch.Stop();
            var successRate = totalDeliveries > 0 ? (double)successCount / totalDeliveries * 100 : 0;

            _logger.LogInformation(
                "[{JobName}] Webhook投递完成。总计: {Total}, 成功: {Success}, 失败: {Fail}, 成功率: {Rate:F2}%, 耗时: {Duration}ms",
                JobName, totalDeliveries, successCount, failCount, successRate, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "[{JobName}] Webhook投递出错，耗时: {Duration}ms",
                JobName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
