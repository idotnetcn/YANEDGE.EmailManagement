using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YANEDGE.EmailManagement.Integrations;
using YANEDGE.EmailManagement.IntegrationApps;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.IntegrationApps;

[Authorize(EmailManagementPermissions.IntegrationApps.Default)]
public class IntegrationAppService : ApplicationService, IIntegrationAppService
{
    private readonly IRepository<IntegrationApp, Guid> _repository;

    public IntegrationAppService(IRepository<IntegrationApp, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<IntegrationAppDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _repository.GetQueryableAsync();
        var total = query.Count();
        var items = query
            .OrderBy(x => x.AppName)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<IntegrationAppDto>(
            total,
            ObjectMapper.Map<List<IntegrationApp>, List<IntegrationAppDto>>(items)
        );
    }

    public async Task<IntegrationAppDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<IntegrationApp, IntegrationAppDto>(entity);
    }

    [Authorize(EmailManagementPermissions.IntegrationApps.Create)]
    public async Task<IntegrationAppDto> CreateAsync(CreateIntegrationAppInput input)
    {
        var secret = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        var entity = new IntegrationApp(GuidGenerator.Create(), input.AppId, input.AppName, secret)
        {
            SourceSystem = input.SourceSystem,
            CallbackUrl = input.CallbackUrl,
            Description = input.Description
        };

        await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<IntegrationApp, IntegrationAppDto>(entity);
    }

    [Authorize(EmailManagementPermissions.IntegrationApps.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id, autoSave: true);
    }
}
