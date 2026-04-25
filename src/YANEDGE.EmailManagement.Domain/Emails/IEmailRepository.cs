using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Emails;

public interface IEmailRepository : IRepository<Email, Guid>
{
    Task<List<Email>> GetListAsync(
        EmailStatus? status = null,
        EmailPriority? priority = null,
        string? fromAddress = null,
        DateTime? scheduledBefore = null,
        int maxResultCount = 10,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        EmailStatus? status = null,
        EmailPriority? priority = null,
        string? fromAddress = null,
        CancellationToken cancellationToken = default);

    Task<List<Email>> GetQueuedEmailsAsync(
        int maxCount = 50,
        CancellationToken cancellationToken = default);
}
