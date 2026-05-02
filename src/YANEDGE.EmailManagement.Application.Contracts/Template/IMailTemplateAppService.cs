using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.Template;

public interface IMailTemplateAppService : IApplicationService
{
    Task<PagedResultDto<MailTemplateDto>> GetListAsync(GetTemplateListInput input);

    Task<MailTemplateDto> GetAsync(Guid id);

    Task<MailTemplateDto> GetByCodeAsync(string code);

    Task<MailTemplateDto> CreateAsync(CreateMailTemplateInput input);

    Task<MailTemplateDto> UpdateAsync(Guid id, UpdateMailTemplateInput input);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task ArchiveAsync(Guid id);

    Task SetAsDefaultAsync(Guid id);

    Task UnsetAsDefaultAsync(Guid id);

    Task DeleteAsync(Guid id);
}
