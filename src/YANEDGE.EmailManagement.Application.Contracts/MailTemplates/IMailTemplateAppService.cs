using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.MailTemplates;

public interface IMailTemplateAppService : IApplicationService
{
    Task<PagedResultDto<MailTemplateDto>> GetListAsync(MailTemplateListRequestDto input);
    Task<MailTemplateDto> GetAsync(Guid id);
    Task<MailTemplateDto> CreateAsync(CreateMailTemplateInput input);
    Task<MailTemplateDto> UpdateAsync(Guid id, UpdateMailTemplateInput input);
    Task DeleteAsync(Guid id);
}
