using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Templates;

public interface IMailTemplateRepository : IRepository<MailTemplate, Guid>
{
    Task<MailTemplate?> FindByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken ct = default);
}
