using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.Emails;

public interface IEmailTemplateAppService : IApplicationService
{
    Task<EmailTemplateDto> GetAsync(Guid id);
    Task<PagedResultDto<EmailTemplateDto>> GetListAsync(GetEmailTemplateListDto input);
    Task<EmailTemplateDto> CreateAsync(CreateEmailTemplateDto input);
    Task<EmailTemplateDto> UpdateAsync(Guid id, UpdateEmailTemplateDto input);
    Task DeleteAsync(Guid id);
    Task<EmailTemplateDto> ActivateAsync(Guid id);
    Task<EmailTemplateDto> DeactivateAsync(Guid id);
}
