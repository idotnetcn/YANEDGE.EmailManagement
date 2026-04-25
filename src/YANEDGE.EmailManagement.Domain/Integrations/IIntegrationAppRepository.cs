using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Integrations;

public interface IIntegrationAppRepository : IRepository<IntegrationApp, Guid>
{
    Task<IntegrationApp?> FindByAppIdAsync(string appId, CancellationToken ct = default);
}
