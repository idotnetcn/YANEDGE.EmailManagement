using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.IntegrationApps;

public interface IIntegrationAppService : IApplicationService
{
    Task<PagedResultDto<IntegrationAppDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    Task<IntegrationAppDto> GetAsync(Guid id);
    Task<IntegrationAppDto> CreateAsync(CreateIntegrationAppInput input);
    Task DeleteAsync(Guid id);
}
