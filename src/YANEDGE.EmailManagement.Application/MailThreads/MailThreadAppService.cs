using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YANEDGE.EmailManagement.Conversations;
using YANEDGE.EmailManagement.MailThreads;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.MailThreads;

[Authorize(EmailManagementPermissions.MailThreads.Default)]
public class MailThreadAppService : ApplicationService, IMailThreadAppService
{
    private readonly IRepository<MailThread, Guid> _repository;

    public MailThreadAppService(IRepository<MailThread, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<MailThreadDto>> GetListAsync(MailThreadListRequestDto input)
    {
        var query = await _repository.GetQueryableAsync();

        query = query
            .WhereIf(input.MailAccountId.HasValue, x => x.MailAccountId == input.MailAccountId!.Value)
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status!.Value)
            .WhereIf(input.OwnerUserId.HasValue, x => x.OwnerUserId == input.OwnerUserId!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Subject != null && x.Subject.Contains(input.Filter!));

        var total = query.Count();
        var items = query
            .OrderByDescending(x => x.LatestMessageTime)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<MailThreadDto>(
            total,
            ObjectMapper.Map<List<MailThread>, List<MailThreadDto>>(items)
        );
    }

    public async Task<MailThreadDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<MailThread, MailThreadDto>(entity);
    }

    [Authorize(EmailManagementPermissions.MailThreads.Archive)]
    public async Task CloseAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        entity.Close();
        await _repository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(EmailManagementPermissions.MailThreads.Archive)]
    public async Task ArchiveAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        entity.Archive();
        await _repository.UpdateAsync(entity, autoSave: true);
    }

    [Authorize(EmailManagementPermissions.MailThreads.Assign)]
    public async Task AssignAsync(Guid id, AssignThreadInput input)
    {
        var entity = await _repository.GetAsync(id);
        entity.Assign(input.ToUserId, input.ToRoleId, input.ToOrganizationUnitId, CurrentUser.Id);
        await _repository.UpdateAsync(entity, autoSave: true);
    }
}
