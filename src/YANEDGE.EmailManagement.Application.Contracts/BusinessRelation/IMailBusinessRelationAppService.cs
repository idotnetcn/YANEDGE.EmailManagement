using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.BusinessRelation;

public interface IMailBusinessRelationAppService : IApplicationService
{
    Task<PagedResultDto<MailBusinessRelationDto>> GetListAsync(GetBusinessRelationListInput input);

    Task<MailBusinessRelationDto> GetAsync(Guid id);

    Task<MailBusinessRelationDto> CreateAsync(CreateMailBusinessRelationInput input);

    Task<MailBusinessRelationDto> UpdateAsync(Guid id, UpdateMailBusinessRelationInput input);

    Task SetAsPrimaryAsync(Guid id);

    Task UnsetAsPrimaryAsync(Guid id);

    Task DeleteAsync(Guid id);
}
