using System.Diagnostics;
using Hangfire;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Domain.Services;

namespace YANEDGE.EmailManagement.Application.BackgroundJobs;

/// <summary>
/// 发件任务处理后台任务
/// </summary>
[DisableConcurrentExecution(timeoutInSeconds: 300)]
[AutomaticRetry(Attempts = 0)]
public class SendTaskProcessorJob : ITransientDependency
{
    private readonly IMailSendTaskRepository _sendTaskRepository;
    private readonly IMailSendService _mailSendService;
    private readonly ILogger<SendTaskProcessorJob> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    private const string JobName = "SendTaskProcessor";
    private const int MaxBatchSize = 50; // 每批次最多处理50个任务

    public SendTaskProcessorJob(
        IMailSendTaskRepository sendTaskRepository,
        IMailSendService mailSendService,
        ILogger<SendTaskProcessorJob> logger,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _sendTaskRepository = sendTaskRepository;
        _mailSendService = mailSendService;
        _logger = logger;
        _unitOfWorkManager = unitOfWorkManager;
    }

    /// <summary>
    /// 执行发件任务处理
    /// </summary>
    public async Task ExecuteAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("[{JobName}] 发件任务处理开始执行...", JobName);

        var successCount = 0;
        var failCount = 0;
        var totalTasks = 0;

        try
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

            // 获取所有待发送的任务
            var pendingTasks = await _sendTaskRepository.GetPendingSendTasksAsync();
            totalTasks = pendingTasks.Count;

            if (totalTasks == 0)
            {
                _logger.LogInformation("[{JobName}] 没有待发送任务", JobName);
                await uow.CompleteAsync();
                return;
            }

            _logger.LogInformation("[{JobName}] 找到 {Count} 个待发送任务", JobName, totalTasks);

            // 限制批次大小，避免长时间运行
            var tasksToProcess = pendingTasks.Take(MaxBatchSize).ToList();
            if (totalTasks > MaxBatchSize)
            {
                _logger.LogWarning("[{JobName}] 任务数量超过批次限制，本次仅处理前 {BatchSize} 个任务",
                    JobName, MaxBatchSize);
            }

            foreach (var task in tasksToProcess)
            {
                try
                {
                    _logger.LogDebug("[{JobName}] 开始处理发件任务: {TaskId}",
                        JobName, task.Id);

                    await _mailSendService.ExecuteSendTaskAsync(task.Id);

                    successCount++;
                    _logger.LogDebug("[{JobName}] 发件任务处理成功: {TaskId}", JobName, task.Id);
                }
                catch (OperationCanceledException)
                {
                    failCount++;
                    _logger.LogWarning("[{JobName}] 发件任务已取消: {TaskId}", JobName, task.Id);
                }
                catch (Exception ex)
                {
                    failCount++;
                    _logger.LogError(ex, "[{JobName}] 发件任务处理失败: {TaskId}", JobName, task.Id);
                    // 继续处理下一个任务，不中断整个批次
                }
            }

            await uow.CompleteAsync();

            stopwatch.Stop();
            var successRate = tasksToProcess.Count > 0 ? (double)successCount / tasksToProcess.Count * 100 : 0;

            _logger.LogInformation(
                "[{JobName}] 发件任务处理完成。总计: {Total}, 处理: {Processed}, 成功: {Success}, 失败: {Fail}, 成功率: {Rate:F2}%, 耗时: {Duration}ms",
                JobName, totalTasks, tasksToProcess.Count, successCount, failCount, successRate, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "[{JobName}] 发件任务处理出错，耗时: {Duration}ms",
                JobName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
