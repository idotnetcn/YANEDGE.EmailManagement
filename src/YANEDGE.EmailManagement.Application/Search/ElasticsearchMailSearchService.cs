using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using YANEDGE.EmailManagement.Domain.MailMessage;
using YANEDGE.EmailManagement.Domain.Search;

namespace YANEDGE.EmailManagement.Application.Search;

/// <summary>
/// Elasticsearch邮件搜索服务实现
/// </summary>
public class ElasticsearchMailSearchService : IMailSearchService, ITransientDependency
{
    private readonly ElasticsearchClient? _client;
    private readonly IMailMessageRepository _messageRepository;
    private readonly ILogger<ElasticsearchMailSearchService> _logger;
    private readonly ElasticsearchOptions _options;
    private const string IndexName = "mail-messages";

    public ElasticsearchMailSearchService(
        IOptions<ElasticsearchOptions> options,
        IMailMessageRepository messageRepository,
        ILogger<ElasticsearchMailSearchService> logger)
    {
        _options = options.Value;
        _messageRepository = messageRepository;
        _logger = logger;

        // Only create client if Elasticsearch is enabled
        if (_options.Enabled && !string.IsNullOrWhiteSpace(_options.Url))
        {
            var settings = new ElasticsearchClientSettings(new Uri(_options.Url));

            if (!string.IsNullOrWhiteSpace(_options.Username) &&
                !string.IsNullOrWhiteSpace(_options.Password))
            {
                settings.Authentication(new Elastic.Transport.BasicAuthentication(
                    _options.Username,
                    _options.Password));
            }

            _client = new ElasticsearchClient(settings);
        }
    }

    public async Task IndexMailMessageAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        if (!IsEnabled())
        {
            _logger.LogDebug("Elasticsearch is disabled, skipping indexing");
            return;
        }

        try
        {
            var message = await _messageRepository.GetAsync(messageId, cancellationToken: cancellationToken);
            var document = MapToDocument(message);

            var response = await _client!.IndexAsync(document, request => request
                .Index(IndexName), cancellationToken);

            if (!response.IsValidResponse)
            {
                _logger.LogError("Failed to index message {MessageId}: {Error}",
                    messageId, response.DebugInformation);
            }
            else
            {
                _logger.LogDebug("Successfully indexed message {MessageId}", messageId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing message {MessageId}", messageId);
        }
    }

    public async Task BulkIndexMailMessagesAsync(IEnumerable<Guid> messageIds, CancellationToken cancellationToken = default)
    {
        if (!IsEnabled())
        {
            _logger.LogDebug("Elasticsearch is disabled, skipping bulk indexing");
            return;
        }

        var messageIdList = messageIds.ToList();
        if (messageIdList.Count == 0)
        {
            return;
        }

        try
        {
            var messages = await _messageRepository.GetListByIdsAsync(messageIdList, cancellationToken);
            var documents = messages.Select(MapToDocument).ToList();

            var bulkResponse = await _client!.BulkAsync(b => b
                .Index(IndexName)
                .IndexMany(documents), cancellationToken);

            if (!bulkResponse.IsValidResponse)
            {
                _logger.LogError("Bulk indexing failed: {Error}", bulkResponse.DebugInformation);
            }
            else
            {
                _logger.LogInformation("Successfully bulk indexed {Count} messages", documents.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk indexing messages");
        }
    }

    public async Task DeleteMailMessageIndexAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        if (!IsEnabled())
        {
            return;
        }

        try
        {
            var response = await _client!.DeleteAsync(IndexName, messageId, cancellationToken);

            if (!response.IsValidResponse && response.Result != Elastic.Clients.Elasticsearch.Result.NotFound)
            {
                _logger.LogError("Failed to delete message index {MessageId}: {Error}",
                    messageId, response.DebugInformation);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting message index {MessageId}", messageId);
        }
    }

    public async Task<MailSearchResult> SearchAsync(MailSearchRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsEnabled())
        {
            _logger.LogWarning("Elasticsearch is disabled, returning empty result");
            return new MailSearchResult { TotalCount = 0, Items = new List<MailSearchDocument>() };
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var mustQueries = new List<Query>();

            // Full-text search across multiple fields
            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                mustQueries.Add(new MultiMatchQuery
                {
                    Query = request.Query,
                    Fields = new[] { "subject^3", "bodyText^2", "fromName", "fromAddress", "attachmentNames" }
                });
            }

            // Filters
            if (request.MailAccountId.HasValue)
            {
                mustQueries.Add(new TermQuery
                {
                    Field = "mailAccountId",
                    Value = FieldValue.String(request.MailAccountId.Value.ToString())
                });
            }

            if (request.ThreadId.HasValue)
            {
                mustQueries.Add(new TermQuery
                {
                    Field = "threadId",
                    Value = FieldValue.String(request.ThreadId.Value.ToString())
                });
            }

            if (!string.IsNullOrWhiteSpace(request.FromAddress))
            {
                mustQueries.Add(new TermQuery
                {
                    Field = "fromAddress",
                    Value = FieldValue.String(request.FromAddress.ToLower())
                });
            }

            if (!string.IsNullOrWhiteSpace(request.ToAddress))
            {
                mustQueries.Add(new TermQuery
                {
                    Field = "toAddresses",
                    Value = FieldValue.String(request.ToAddress.ToLower())
                });
            }

            if (request.HasAttachments.HasValue)
            {
                mustQueries.Add(new TermQuery
                {
                    Field = "hasAttachments",
                    Value = FieldValue.Boolean(request.HasAttachments.Value)
                });
            }

            if (request.StartDate.HasValue || request.EndDate.HasValue)
            {
                var rangeQuery = new DateRangeQuery("receivedTime");
                if (request.StartDate.HasValue)
                {
                    rangeQuery.Gte = request.StartDate.Value;
                }
                if (request.EndDate.HasValue)
                {
                    rangeQuery.Lte = request.EndDate.Value;
                }
                mustQueries.Add(rangeQuery);
            }

            if (request.Labels != null && request.Labels.Count > 0)
            {
                mustQueries.Add(new TermsQuery
                {
                    Field = "labels",
                    Terms = new TermsQueryField(request.Labels.Select(l => FieldValue.String(l)).ToArray())
                });
            }

            var searchResponse = await _client!.SearchAsync<MailSearchDocument>(s => s
                .Index(IndexName)
                .Query(q => q
                    .Bool(b => b
                        .Must(mustQueries.ToArray())))
                .From(request.Skip)
                .Size(request.Take)
                .Sort(GetSortOptions(request)),
                cancellationToken);

            stopwatch.Stop();

            if (!searchResponse.IsValidResponse)
            {
                _logger.LogError("Search failed: {Error}", searchResponse.DebugInformation);
                return new MailSearchResult { TotalCount = 0, Items = new List<MailSearchDocument>() };
            }

            var items = searchResponse.Documents.Select((doc, index) =>
            {
                doc.Score = (float)(searchResponse.Hits.ElementAt(index).Score ?? 0);
                return doc;
            }).ToList();

            return new MailSearchResult
            {
                TotalCount = searchResponse.Total,
                Items = items,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error searching messages");
            return new MailSearchResult { TotalCount = 0, Items = new List<MailSearchDocument>() };
        }
    }

    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        if (!IsEnabled())
        {
            return false;
        }

        try
        {
            var pingResponse = await _client!.PingAsync(cancellationToken);
            return pingResponse.IsValidResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Elasticsearch health check failed");
            return false;
        }
    }

    public async Task EnsureIndexAsync(CancellationToken cancellationToken = default)
    {
        if (!IsEnabled())
        {
            return;
        }

        try
        {
            var existsResponse = await _client!.Indices.ExistsAsync(IndexName, cancellationToken);

            if (!existsResponse.Exists)
            {
                var createResponse = await _client.Indices.CreateAsync(IndexName, c => c
                    .Mappings(m => m
                        .Properties<MailSearchDocument>(p => p
                            .Keyword(k => k.Id)
                            .Keyword(k => k.ThreadId)
                            .Keyword(k => k.MailAccountId)
                            .Text(t => t.Subject, td => td.Analyzer("standard"))
                            .Keyword(k => k.FromAddress)
                            .Text(t => t.FromName)
                            .Keyword(k => k.ToAddresses)
                            .Keyword(k => k.CcAddresses)
                            .Text(t => t.BodyPreview)
                            .Text(t => t.BodyText, td => td.Analyzer("standard"))
                            .Boolean(b => b.HasAttachments)
                            .Keyword(k => k.AttachmentNames)
                            .Keyword(k => k.Labels)
                            .Date(d => d.ReceivedTime)
                            .Date(d => d.IndexedAt))),
                    cancellationToken);

                if (!createResponse.IsValidResponse)
                {
                    _logger.LogError("Failed to create index: {Error}", createResponse.DebugInformation);
                }
                else
                {
                    _logger.LogInformation("Successfully created index: {IndexName}", IndexName);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ensuring index exists");
        }
    }

    private bool IsEnabled()
    {
        return _options.Enabled && _client != null;
    }

    private MailSearchDocument MapToDocument(Domain.MailMessage.MailMessage message)
    {
        return new MailSearchDocument
        {
            Id = message.Id,
            ThreadId = message.ThreadId ?? Guid.Empty,
            MailAccountId = message.MailAccountId,
            Subject = message.Subject ?? string.Empty,
            FromAddress = message.FromAddress ?? string.Empty,
            FromName = message.FromDisplayName ?? string.Empty,
            ToAddresses = new List<string>(), // MailMessage doesn't have ToAddresses
            CcAddresses = new List<string>(), // MailMessage doesn't have CcAddresses
            BodyPreview = TruncateText(message.TextBody ?? message.SanitizedHtmlBody, 500),
            BodyText = message.TextBody ?? StripHtml(message.SanitizedHtmlBody),
            HasAttachments = message.HasAttachment,
            AttachmentNames = new List<string>(), // TODO: Get from attachments
            Labels = new List<string>(), // TODO: Get from labels
            ReceivedTime = message.ReceivedTime ?? message.SentTime ?? message.CreatedAt,
            IndexedAt = DateTime.UtcNow
        };
    }

    private List<string> ParseEmailList(string? emailList)
    {
        if (string.IsNullOrWhiteSpace(emailList))
        {
            return new List<string>();
        }

        return emailList.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(e => e.Trim().ToLower())
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .ToList();
    }

    private string TruncateText(string? text, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
    }

    private string StripHtml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        // Simple HTML stripping - for production, consider using HtmlAgilityPack
        return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
    }

    private Action<SortOptionsDescriptor<MailSearchDocument>>[] GetSortOptions(MailSearchRequest request)
    {
        var sortOptions = new List<Action<SortOptionsDescriptor<MailSearchDocument>>>();

        if (!string.IsNullOrWhiteSpace(request.SortField))
        {
            var sortOrder = request.SortDirection?.ToLower() == "asc"
                ? SortOrder.Asc
                : SortOrder.Desc;

            sortOptions.Add(s =>
            {
                switch (request.SortField.ToLower())
                {
                    case "subject":
                        s.Field(f => f.Subject, sortOrder);
                        break;
                    case "fromaddress":
                        s.Field(f => f.FromAddress, sortOrder);
                        break;
                    case "receivedtime":
                        s.Field(f => f.ReceivedTime, sortOrder);
                        break;
                    default:
                        s.Field(f => f.ReceivedTime, SortOrder.Desc);
                        break;
                }
            });
        }
        else
        {
            // Default sort by received time descending
            sortOptions.Add(s => s.Field(f => f.ReceivedTime, SortOrder.Desc));
        }

        return sortOptions.ToArray();
    }
}

/// <summary>
/// Elasticsearch配置选项
/// </summary>
public class ElasticsearchOptions
{
    public bool Enabled { get; set; } = false;
    public string Url { get; set; } = "http://localhost:9200";
    public string? Username { get; set; }
    public string? Password { get; set; }
}
