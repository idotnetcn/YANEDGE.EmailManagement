using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Conversations;

public interface IMailMessageRepository : IRepository<MailMessage, Guid>
{
    Task<MailMessage?> FindByExternalUidAsync(Guid mailAccountId, string externalUid, byte folderType, CancellationToken ct = default);
}
