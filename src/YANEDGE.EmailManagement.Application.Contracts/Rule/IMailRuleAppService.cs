using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.Rule;

public interface IMailRuleAppService : IApplicationService
{
    Task<PagedResultDto<MailRuleDto>> GetListAsync(GetRuleListInput input);

    Task<MailRuleDto> GetAsync(Guid id);

    Task<MailRuleDto> CreateAsync(CreateMailRuleInput input);

    Task<MailRuleDto> UpdateAsync(Guid id, UpdateMailRuleInput input);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);

    Task DeleteAsync(Guid id);
}
