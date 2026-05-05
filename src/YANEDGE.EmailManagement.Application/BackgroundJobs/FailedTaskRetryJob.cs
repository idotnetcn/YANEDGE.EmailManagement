using System.Diagnostics;
using Hangfire;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Domain.Services;

namespace YANEDGE.EmailManagement.Application.BackgroundJobs;

/// <summary>
/// 失败任务重试后台任务
/// </summary>
[DisableConcurrentExecution(timeoutInSeconds: 300)]
[AutomaticRetry(Attempts = 0)]
public class FailedTaskRetryJob : ITransientDependency
{
    private readonly IMailSendTaskRepository _sendTaskRepository;
    private readonly IMailSendService _mailSendService;
    private readonly ILogger<FailedTaskRetryJob> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    private const string JobName = "FailedTaskRetry";
    private const int MaxRetryBatchSize = 20; // 每批次最多重试20个任务

    public FailedTaskRetryJob(
        IMailSendTaskRepository sendTaskRepository,
        IMailSendService mailSendService,
        ILogger<FailedTaskRetryJob> logger,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _sendTaskRepository = sendTaskRepository;
        _mailSendService = mailSendService;
        _logger = logger;
        _unitOfWorkManager = unitOfWorkManager;
    }

    /// <summary>
    /// 执行失败任务重试
    /// </summary>
    public async Task ExecuteAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("[{JobName}] 失败任务重试开始执行...", JobName);

        var retrySuccessCount = 0;
        var retryFailCount = 0;
        var totalTasks = 0;

        try
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

            // 获取所有可重试的失败任务
            var failedTasks = await _sendTaskRepository.GetFailedTasksForRetryAsync();
            totalTasks = failedTasks.Count;

            if (totalTasks == 0)
            {
                _logger.LogInformation("[{JobName}] 没有可重试的失败任务", JobName);
                await uow.CompleteAsync();
                return;
            }

            _logger.LogInformation("[{JobName}] 找到 {Count} 个可重试的失败任务", JobName, totalTasks);

            // 限制批次大小
            var tasksToRetry = failedTasks.Take(MaxRetryBatchSize).ToList();
            if (totalTasks > MaxRetryBatchSize)
            {
                _logger.LogWarning("[{JobName}] 重试任务数量超过批次限制，本次仅处理前 {BatchSize} 个任务",
                    JobName, MaxRetryBatchSize);
            }

            foreach (var task in tasksToRetry)
            {
                try
                {
                    _logger.LogDebug(
                        "[{JobName}] 开始重试任务: {TaskId}, 当前重试次数: {RetryCount}/{MaxRetry}",
                        JobName,
                        task.Id,
                        task.RetryCount,
                        task.MaxRetryCount);

                    task.Retry();
                    await _sendTaskRepository.UpdateAsync(task);

                    await _mailSendService.ExecuteSendTaskAsync(task.Id);

                    retrySuccessCount++;
                    _logger.LogInformation("[{JobName}] 任务重试成功: {TaskId}", JobName, task.Id);
                }
                catch (OperationCanceledException)
                {
                    retryFailCount++;
                    _logger.LogWarning("[{JobName}] 任务重试已取消: {TaskId}", JobName, task.Id);
                }
                catch (Exception ex)
                {
                    retryFailCount++;
                    _logger.LogError(ex, "[{JobName}] 任务重试失败: {TaskId}, 重试次数: {RetryCount}/{MaxRetry}",
                        JobName, task.Id, task.RetryCount, task.MaxRetryCount);

                    // 检查是否已达最大重试次数
                    if (task.RetryCount >= task.MaxRetryCount)
                    {
                        _logger.LogWarning("[{JobName}] 任务 {TaskId} 已达最大重试次数，将不再重试",
                            JobName, task.Id);
                    }
                    // 继续处理下一个任务
                }
            }

            await uow.CompleteAsync();

            stopwatch.Stop();
            var successRate = tasksToRetry.Count > 0 ? (double)retrySuccessCount / tasksToRetry.Count * 100 : 0;

            _logger.LogInformation(
                "[{JobName}] 失败任务重试完成。总计: {Total}, 处理: {Processed}, 重试成功: {Success}, 重试失败: {Fail}, 成功率: {Rate:F2}%, 耗时: {Duration}ms",
                JobName, totalTasks, tasksToRetry.Count, retrySuccessCount, retryFailCount, successRate, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "[{JobName}] 失败任务重试出错，耗时: {Duration}ms",
                JobName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
