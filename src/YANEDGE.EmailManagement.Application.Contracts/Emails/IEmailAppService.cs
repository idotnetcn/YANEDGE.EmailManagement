using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.Emails;

public interface IEmailAppService : IApplicationService
{
    Task<EmailDto> GetAsync(Guid id);
    Task<PagedResultDto<EmailDto>> GetListAsync(GetEmailListDto input);
    Task<EmailDto> CreateAsync(CreateEmailDto input);
    Task<EmailDto> UpdateAsync(Guid id, UpdateEmailDto input);
    Task DeleteAsync(Guid id);
    Task<EmailDto> QueueAsync(Guid id, QueueEmailDto input);
    Task<EmailDto> CancelAsync(Guid id);
}
