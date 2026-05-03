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
        _logger.LogInformation("规则执行任务开始...");

        try
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

            // 获取所有激活的规则
            var activeRules = await _ruleRepository.GetActiveRulesOrderedByPriorityAsync();

            if (!activeRules.Any())
            {
                _logger.LogInformation("没有激活的规则，跳过执行");
                await uow.CompleteAsync();
                return;
            }

            _logger.LogInformation("找到 {Count} 个激活的规则", activeRules.Count);

            // 获取最近未处理的邮件（例如最近1小时内收到的）
            var recentMessages = await _messageRepository.GetListAsync();
            var unprocessedMessages = recentMessages
                .Where(m => m.ReceivedTime.HasValue && m.ReceivedTime.Value >= DateTime.UtcNow.AddHours(-1))
                .ToList();

            _logger.LogInformation("找到 {Count} 个待处理邮件", unprocessedMessages.Count);

            var processedCount = 0;

            foreach (var message in unprocessedMessages)
            {
                try
                {
                    await _ruleMatchingService.ExecuteMatchingRulesAsync(activeRules, message);
                    processedCount++;
                    _logger.LogDebug("邮件规则处理完成: {MessageId}", message.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "邮件规则处理失败: {MessageId}", message.Id);
                }
            }

            await uow.CompleteAsync();

            _logger.LogInformation("规则执行任务完成。处理邮件数: {Count}", processedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "规则执行任务出错");
            throw;
        }
    }
}
