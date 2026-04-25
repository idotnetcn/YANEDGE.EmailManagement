using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.Accounts;

public interface IMailAccountRepository : IRepository<MailAccount, Guid>
{
    Task<MailAccount?> FindByEmailAddressAsync(string emailAddress, CancellationToken ct = default);
    Task<bool> EmailAddressExistsAsync(string emailAddress, Guid? excludeId = null, CancellationToken ct = default);
}
