using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using YANEDGE.EmailManagement.Domain.MailCompose;
using YANEDGE.EmailManagement.Domain.Services;

namespace YANEDGE.EmailManagement.Application.BackgroundJobs;

/// <summary>
/// 发件任务处理后台任务
/// </summary>
public class SendTaskProcessorJob : ITransientDependency
{
    private readonly IMailSendTaskRepository _sendTaskRepository;
    private readonly IMailSendService _mailSendService;
    private readonly ILogger<SendTaskProcessorJob> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

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
        _logger.LogInformation("发件任务处理开始执行...");

        try
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

            // 获取所有待发送的任务
            var pendingTasks = await _sendTaskRepository.GetPendingSendTasksAsync();

            _logger.LogInformation("找到 {Count} 个待发送任务", pendingTasks.Count);

            var successCount = 0;
            var failCount = 0;

            foreach (var task in pendingTasks)
            {
                try
                {
                    _logger.LogDebug("开始处理发件任务: {TaskId}", task.Id);

                    await _mailSendService.ExecuteSendTaskAsync(task.Id);

                    successCount++;
                    _logger.LogDebug("发件任务处理成功: {TaskId}", task.Id);
                }
                catch (Exception ex)
                {
                    failCount++;
                    _logger.LogError(ex, "发件任务处理失败: {TaskId}", task.Id);
                }
            }

            await uow.CompleteAsync();

            _logger.LogInformation(
                "发件任务处理完成。成功: {Success}, 失败: {Fail}",
                successCount,
                failCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发件任务处理出错");
            throw;
        }
    }
}
