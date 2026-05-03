using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Domain.Services;

namespace YANEDGE.EmailManagement.Application.BackgroundJobs;

/// <summary>
/// 失败任务重试后台任务
/// </summary>
public class FailedTaskRetryJob : ITransientDependency
{
    private readonly IMailSendTaskRepository _sendTaskRepository;
    private readonly IMailSendService _mailSendService;
    private readonly ILogger<FailedTaskRetryJob> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

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
        _logger.LogInformation("失败任务重试开始执行...");

        try
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

            // 获取所有可重试的失败任务
            var failedTasks = await _sendTaskRepository.GetFailedTasksForRetryAsync();

            _logger.LogInformation("找到 {Count} 个可重试的失败任务", failedTasks.Count);

            var retrySuccessCount = 0;
            var retryFailCount = 0;

            foreach (var task in failedTasks)
            {
                try
                {
                    _logger.LogDebug(
                        "开始重试任务: {TaskId}, 当前重试次数: {RetryCount}/{MaxRetry}",
                        task.Id,
                        task.RetryCount,
                        task.MaxRetryCount);

                    await _mailSendService.ExecuteSendTaskAsync(task.Id);

                    retrySuccessCount++;
                    _logger.LogInformation("任务重试成功: {TaskId}", task.Id);
                }
                catch (Exception ex)
                {
                    retryFailCount++;
                    _logger.LogError(ex, "任务重试失败: {TaskId}", task.Id);
                }
            }

            await uow.CompleteAsync();

            _logger.LogInformation(
                "失败任务重试完成。重试成功: {Success}, 重试失败: {Fail}",
                retrySuccessCount,
                retryFailCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "失败任务重试出错");
            throw;
        }
    }
}
