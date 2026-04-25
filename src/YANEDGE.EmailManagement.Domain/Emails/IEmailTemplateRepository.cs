using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Emails;

public interface IEmailTemplateRepository : IRepository<EmailTemplate, Guid>
{
    Task<EmailTemplate?> FindByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<List<EmailTemplate>> GetListAsync(
        bool? isActive = null,
        string? filter = null,
        int maxResultCount = 10,
        int skipCount = 0,
        CancellationToken cancellationToken = default);
}
