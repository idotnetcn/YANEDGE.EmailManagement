using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using YANEDGE.EmailManagement.Domain.MailMessage;
using YANEDGE.EmailManagement.Domain.Rule;
using YANEDGE.EmailManagement.Domain.Services;

namespace YANEDGE.EmailManagement.Application.BackgroundJobs;

/// <summary>
/// 规则执行后台任务
/// </summary>
public class RuleExecutionJob : ITransientDependency
{
    private readonly IMailMessageRepository _messageRepository;
    private readonly IMailRuleRepository _ruleRepository;
    private readonly IRuleMatchingService _ruleMatchingService;
    private readonly ILogger<RuleExecutionJob> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    private const string JobName = "RuleExecution";
    private const int LookbackHours = 1; // 处理最近1小时的邮件
    private const int MaxMessagesPerRun = 100; // 每次最多处理100封邮件

    public RuleExecutionJob(
        IMailMessageRepository messageRepository,
        IMailRuleRepository ruleRepository,
        IRuleMatchingService ruleMatchingService,
        ILogger<RuleExecutionJob> logger,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _messageRepository = messageRepository;
        _ruleRepository = ruleRepository;
        _ruleMatchingService = ruleMatchingService;
        _logger = logger;
        _unitOfWorkManager = unitOfWorkManager;
    }

    /// <summary>
    /// 执行规则匹配任务
    /// 对最近收到的未处理邮件进行规则匹配
    /// </summary>
    public async Task ExecuteAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("[{JobName}] 规则执行任务开始...", JobName);

        var processedCount = 0;
        var failedCount = 0;
        var totalMessages = 0;

        try
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

            // 获取所有激活的规则
            var activeRules = await _ruleRepository.GetActiveRulesOrderedByPriorityAsync();

            if (!activeRules.Any())
            {
                _logger.LogInformation("[{JobName}] 没有激活的规则，跳过执行", JobName);
                await uow.CompleteAsync();
                return;
            }

            _logger.LogInformation("[{JobName}] 找到 {Count} 个激活的规则", JobName, activeRules.Count);

            // 获取最近未处理的邮件
            var cutoffTime = DateTime.UtcNow.AddHours(-LookbackHours);
            var recentMessages = await _messageRepository.GetListAsync();
            var unprocessedMessages = recentMessages
                .Where(m => m.ReceivedTime.HasValue && m.ReceivedTime.Value >= cutoffTime)
                .OrderByDescending(m => m.ReceivedTime)
                .Take(MaxMessagesPerRun)
                .ToList();

            totalMessages = unprocessedMessages.Count;

            if (totalMessages == 0)
            {
                _logger.LogInformation("[{JobName}] 没有待处理邮件", JobName);
                await uow.CompleteAsync();
                return;
            }

            _logger.LogInformation("[{JobName}] 找到 {Count} 个待处理邮件（最近 {Hours} 小时）",
                JobName, totalMessages, LookbackHours);

            foreach (var message in unprocessedMessages)
            {
                try
                {
                    _logger.LogDebug("[{JobName}] 开始处理邮件: {MessageId}, 主题: {Subject}",
                        JobName, message.Id, message.Subject);

                    await _ruleMatchingService.ExecuteMatchingRulesAsync(activeRules, message);
                    processedCount++;

                    _logger.LogDebug("[{JobName}] 邮件规则处理完成: {MessageId}", JobName, message.Id);
                }
                catch (OperationCanceledException)
                {
                    failedCount++;
                    _logger.LogWarning("[{JobName}] 邮件规则处理已取消: {MessageId}", JobName, message.Id);
                }
                catch (Exception ex)
                {
                    failedCount++;
                    _logger.LogError(ex, "[{JobName}] 邮件规则处理失败: {MessageId}", JobName, message.Id);
                    // 继续处理下一封邮件
                }
            }

            await uow.CompleteAsync();

            stopwatch.Stop();
            var successRate = totalMessages > 0 ? (double)processedCount / totalMessages * 100 : 0;

            _logger.LogInformation(
                "[{JobName}] 规则执行任务完成。总计: {Total}, 成功: {Success}, 失败: {Failed}, 成功率: {Rate:F2}%, 耗时: {Duration}ms",
                JobName, totalMessages, processedCount, failedCount, successRate, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "[{JobName}] 规则执行任务出错，耗时: {Duration}ms",
                JobName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
