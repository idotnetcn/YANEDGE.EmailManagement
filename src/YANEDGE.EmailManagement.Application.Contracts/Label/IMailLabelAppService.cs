using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.Label;

public interface IMailLabelAppService : IApplicationService
{
    Task<PagedResultDto<MailLabelDto>> GetListAsync(GetLabelListInput input);

    Task<MailLabelDto> GetAsync(Guid id);

    Task<MailLabelDto> CreateAsync(CreateMailLabelInput input);

    Task<MailLabelDto> UpdateAsync(Guid id, UpdateMailLabelInput input);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task SetSortOrderAsync(Guid id, int sortOrder);

    Task DeleteAsync(Guid id);
}
