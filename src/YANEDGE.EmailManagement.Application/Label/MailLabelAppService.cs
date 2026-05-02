using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.Label;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace YANEDGE.EmailManagement.Application.Label;

[Authorize(EmailManagementPermissions.Labels.Default)]
public class MailLabelAppService : ApplicationService, IMailLabelAppService
{
    private readonly IRepository<Domain.Label.MailLabel, Guid> _labelRepository;

    public MailLabelAppService(
        IRepository<Domain.Label.MailLabel, Guid> labelRepository)
    {
        _labelRepository = labelRepository;
    }

    public async Task<PagedResultDto<MailLabelDto>> GetListAsync(GetLabelListInput input)
    {
        var query = await _labelRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            query = query.Where(x => x.Name.Contains(input.Keyword) || (x.Description != null && x.Description.Contains(input.Keyword)));
        }

        if (input.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == input.IsActive.Value);
        }

        if (input.IsSystemLabel.HasValue)
        {
            query = query.Where(x => x.IsSystemLabel == input.IsSystemLabel.Value);
        }

        if (input.OwnerUserId.HasValue)
        {
            query = query.Where(x => x.OwnerUserId == input.OwnerUserId.Value);
        }

        if (input.OwnerOrganizationId.HasValue)
        {
            query = query.Where(x => x.OwnerOrganizationId == input.OwnerOrganizationId.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query.OrderBy(x => x.SortOrder).ThenBy(x => x.Name)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<MailLabelDto>(
            totalCount,
            ObjectMapper.Map<System.Collections.Generic.List<Domain.Label.MailLabel>, System.Collections.Generic.List<MailLabelDto>>(items)
        );
    }

    public async Task<MailLabelDto> GetAsync(Guid id)
    {
        var label = await _labelRepository.GetAsync(id);
        return ObjectMapper.Map<Domain.Label.MailLabel, MailLabelDto>(label);
    }

    [Authorize(EmailManagementPermissions.Labels.Create)]
    public async Task<MailLabelDto> CreateAsync(CreateMailLabelInput input)
    {
        var label = new Domain.Label.MailLabel(
            GuidGenerator.Create(),
            input.Name,
            input.Color,
            input.OwnerUserId,
            input.OwnerOrganizationId,
            input.Description,
            input.IsSystemLabel
        );

        await _labelRepository.InsertAsync(label);

        return ObjectMapper.Map<Domain.Label.MailLabel, MailLabelDto>(label);
    }

    [Authorize(EmailManagementPermissions.Labels.Update)]
    public async Task<MailLabelDto> UpdateAsync(Guid id, UpdateMailLabelInput input)
    {
        var label = await _labelRepository.GetAsync(id);

        label.Update(input.Name, input.Color, input.Description);

        await _labelRepository.UpdateAsync(label);

        return ObjectMapper.Map<Domain.Label.MailLabel, MailLabelDto>(label);
    }

    [Authorize(EmailManagementPermissions.Labels.Update)]
    public async Task ActivateAsync(Guid id)
    {
        var label = await _labelRepository.GetAsync(id);
        label.Activate();
        await _labelRepository.UpdateAsync(label);
    }

    [Authorize(EmailManagementPermissions.Labels.Update)]
    public async Task DeactivateAsync(Guid id)
    {
        var label = await _labelRepository.GetAsync(id);
        label.Deactivate();
        await _labelRepository.UpdateAsync(label);
    }

    [Authorize(EmailManagementPermissions.Labels.Update)]
    public async Task SetSortOrderAsync(Guid id, int sortOrder)
    {
        var label = await _labelRepository.GetAsync(id);
        label.SetSortOrder(sortOrder);
        await _labelRepository.UpdateAsync(label);
    }

    [Authorize(EmailManagementPermissions.Labels.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _labelRepository.DeleteAsync(id);
    }
}
