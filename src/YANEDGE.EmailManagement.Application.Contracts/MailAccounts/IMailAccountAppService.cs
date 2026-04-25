using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.MailAccounts;

public interface IMailAccountAppService : IApplicationService
{
    Task<PagedResultDto<MailAccountDto>> GetListAsync(MailAccountListRequestDto input);
    Task<MailAccountDto> GetAsync(Guid id);
    Task<MailAccountDto> CreateAsync(CreateMailAccountInput input);
    Task<MailAccountDto> UpdateAsync(Guid id, UpdateMailAccountInput input);
    Task DeleteAsync(Guid id);
    Task EnableSyncAsync(Guid id);
    Task DisableSyncAsync(Guid id);
    Task EnableSendAsync(Guid id);
    Task DisableSendAsync(Guid id);
}
