using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Conversations;

public interface IMailThreadRepository : IRepository<MailThread, Guid>
{
    Task<MailThread?> FindByThreadKeyAsync(Guid mailAccountId, string threadKey, CancellationToken ct = default);
}
