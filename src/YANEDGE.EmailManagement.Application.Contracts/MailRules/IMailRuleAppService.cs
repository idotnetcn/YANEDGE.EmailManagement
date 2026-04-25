using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.MailRules;

public interface IMailRuleAppService : IApplicationService
{
    Task<PagedResultDto<MailRuleDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    Task<MailRuleDto> GetAsync(Guid id);
    Task<MailRuleDto> CreateAsync(CreateUpdateMailRuleInput input);
    Task<MailRuleDto> UpdateAsync(Guid id, CreateUpdateMailRuleInput input);
    Task DeleteAsync(Guid id);
}
