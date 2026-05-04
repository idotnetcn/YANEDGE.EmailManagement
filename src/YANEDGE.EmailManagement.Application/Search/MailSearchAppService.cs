using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using YANEDGE.EmailManagement.Application.BackgroundJobs;
using YANEDGE.EmailManagement.Application.Contracts.Search;
using YANEDGE.EmailManagement.Domain.Search;

namespace YANEDGE.EmailManagement.Application.Search;

/// <summary>
/// 邮件搜索应用服务实现
/// </summary>
public class MailSearchAppService : ApplicationService, IMailSearchAppService
{
    private readonly IMailSearchService _searchService;
    private readonly SearchIndexSyncJob _syncJob;
    private readonly ILogger<MailSearchAppService> _logger;

    public MailSearchAppService(
        IMailSearchService searchService,
        SearchIndexSyncJob syncJob,
        ILogger<MailSearchAppService> logger)
    {
        _searchService = searchService;
        _syncJob = syncJob;
        _logger = logger;
    }

    public async Task<PagedResultDto<MailSearchResultDto>> SearchAsync(MailSearchInput input)
    {
        var request = new MailSearchRequest
        {
            Query = input.Query,
            MailAccountId = input.MailAccountId,
            ThreadId = input.ThreadId,
            FromAddress = input.FromAddress,
            ToAddress = input.ToAddress,
            HasAttachments = input.HasAttachments,
            StartDate = input.StartDate,
            EndDate = input.EndDate,
            Labels = input.Labels,
            Skip = input.SkipCount,
            Take = input.MaxResultCount,
            SortField = input.Sorting,
            SortDirection = "desc"
        };

        var result = await _searchService.SearchAsync(request);

        var items = result.Items.Select(doc => new MailSearchResultDto
        {
            Id = doc.Id,
            ThreadId = doc.ThreadId,
            MailAccountId = doc.MailAccountId,
            Subject = doc.Subject,
            FromAddress = doc.FromAddress,
            FromName = doc.FromName,
            ToAddresses = doc.ToAddresses,
            BodyPreview = doc.BodyPreview,
            HasAttachments = doc.HasAttachments,
            AttachmentNames = doc.AttachmentNames,
            Labels = doc.Labels,
            ReceivedTime = doc.ReceivedTime,
            Score = doc.Score
        }).ToList();

        _logger.LogInformation(
            "Search completed: Query={Query}, Results={Count}/{Total}, Time={Time}ms",
            input.Query, items.Count, result.TotalCount, result.ElapsedMilliseconds);

        return new PagedResultDto<MailSearchResultDto>(result.TotalCount, items);
    }

    public async Task<SearchHealthDto> GetHealthAsync()
    {
        var isHealthy = await _searchService.IsHealthyAsync();

        return new SearchHealthDto
        {
            IsAvailable = isHealthy,
            IsEnabled = isHealthy, // If healthy, it's also enabled
            Status = isHealthy ? "Healthy" : "Unavailable"
        };
    }

    public async Task TriggerIndexSyncAsync(IndexSyncType syncType)
    {
        _logger.LogInformation("Triggering {SyncType} index sync", syncType);

        if (syncType == IndexSyncType.Full)
        {
            await _syncJob.ExecuteFullSyncAsync();
        }
        else
        {
            // Incremental sync for last 24 hours
            await _syncJob.ExecuteIncrementalSyncAsync(DateTime.UtcNow.AddDays(-1));
        }
    }
}
