using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using YANEDGE.EmailManagement.Domain.MailMessage;
using YANEDGE.EmailManagement.Domain.Search;

namespace YANEDGE.EmailManagement.Application.BackgroundJobs;

/// <summary>
/// Elasticsearch搜索索引同步后台任务
/// Syncs mail messages to Elasticsearch for search indexing
/// </summary>
public class SearchIndexSyncJob : ITransientDependency
{
    private readonly IMailSearchService _searchService;
    private readonly IMailMessageRepository _messageRepository;
    private readonly ILogger<SearchIndexSyncJob> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    private const string JobName = "SearchIndexSync";
    private const int BatchSize = 100; // 每批次索引100条消息

    public SearchIndexSyncJob(
        IMailSearchService searchService,
        IMailMessageRepository messageRepository,
        ILogger<SearchIndexSyncJob> logger,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _searchService = searchService;
        _messageRepository = messageRepository;
        _logger = logger;
        _unitOfWorkManager = unitOfWorkManager;
    }

    /// <summary>
    /// 执行全量索引同步
    /// </summary>
    public async Task ExecuteFullSyncAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("[{JobName}] 全量索引同步开始执行...", JobName);

        try
        {
            // 确保索引存在
            await _searchService.EnsureIndexAsync();

            // 检查Elasticsearch健康状态
            var isHealthy = await _searchService.IsHealthyAsync();
            if (!isHealthy)
            {
                _logger.LogWarning("[{JobName}] Elasticsearch不可用，取消同步", JobName);
                return;
            }

            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

            // 获取所有消息ID（分页处理）
            var skipCount = 0;
            var totalIndexed = 0;
            var hasMore = true;

            while (hasMore)
            {
                var queryable = await _messageRepository.GetQueryableAsync();
                var messageIds = queryable
                    .OrderBy(m => m.CreationTime)
                    .Skip(skipCount)
                    .Take(BatchSize)
                    .Select(m => m.Id)
                    .ToList();

                if (messageIds.Count == 0)
                {
                    hasMore = false;
                    break;
                }

                _logger.LogDebug(
                    "[{JobName}] 正在索引第 {Start}-{End} 条消息",
                    JobName,
                    skipCount + 1,
                    skipCount + messageIds.Count);

                await _searchService.BulkIndexMailMessagesAsync(messageIds);

                totalIndexed += messageIds.Count;
                skipCount += BatchSize;

                if (messageIds.Count < BatchSize)
                {
                    hasMore = false;
                }
            }

            await uow.CompleteAsync();

            stopwatch.Stop();
            _logger.LogInformation(
                "[{JobName}] 全量索引同步完成。总计索引: {Total}, 耗时: {Duration}ms",
                JobName, totalIndexed, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "[{JobName}] 全量索引同步出错，耗时: {Duration}ms",
                JobName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    /// <summary>
    /// 执行增量索引同步（索引最近的消息）
    /// </summary>
    public async Task ExecuteIncrementalSyncAsync(DateTime since)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation(
            "[{JobName}] 增量索引同步开始执行，时间范围: {Since}",
            JobName, since);

        try
        {
            // 检查Elasticsearch健康状态
            var isHealthy = await _searchService.IsHealthyAsync();
            if (!isHealthy)
            {
                _logger.LogWarning("[{JobName}] Elasticsearch不可用，取消同步", JobName);
                return;
            }

            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

            var queryable = await _messageRepository.GetQueryableAsync();
            var messageIds = queryable
                .Where(m => m.CreationTime >= since)
                .OrderBy(m => m.CreationTime)
                .Select(m => m.Id)
                .ToList();

            if (messageIds.Count == 0)
            {
                _logger.LogInformation("[{JobName}] 没有需要索引的新消息", JobName);
                await uow.CompleteAsync();
                return;
            }

            _logger.LogInformation(
                "[{JobName}] 找到 {Count} 条新消息需要索引",
                JobName, messageIds.Count);

            // 分批处理
            var batches = messageIds.Chunk(BatchSize).ToList();
            var totalIndexed = 0;

            foreach (var batch in batches)
            {
                await _searchService.BulkIndexMailMessagesAsync(batch);
                totalIndexed += batch.Length;

                _logger.LogDebug(
                    "[{JobName}] 已索引 {Current}/{Total} 条消息",
                    JobName, totalIndexed, messageIds.Count);
            }

            await uow.CompleteAsync();

            stopwatch.Stop();
            _logger.LogInformation(
                "[{JobName}] 增量索引同步完成。总计索引: {Total}, 耗时: {Duration}ms",
                JobName, totalIndexed, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "[{JobName}] 增量索引同步出错，耗时: {Duration}ms",
                JobName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
