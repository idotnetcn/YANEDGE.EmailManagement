using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.MailThreads;

public interface IMailThreadAppService : IApplicationService
{
    Task<PagedResultDto<MailThreadDto>> GetListAsync(MailThreadListRequestDto input);
    Task<MailThreadDto> GetAsync(Guid id);
    Task CloseAsync(Guid id);
    Task ArchiveAsync(Guid id);
    Task AssignAsync(Guid id, AssignThreadInput input);
}
