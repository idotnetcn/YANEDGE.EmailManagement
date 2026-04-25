using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YANEDGE.EmailManagement.Templates;
using YANEDGE.EmailManagement.MailTemplates;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace YANEDGE.EmailManagement.MailTemplates;

[Authorize(EmailManagementPermissions.MailTemplates.Default)]
public class MailTemplateAppService : ApplicationService, IMailTemplateAppService
{
    private readonly IRepository<MailTemplate, Guid> _repository;

    public MailTemplateAppService(IRepository<MailTemplate, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<MailTemplateDto>> GetListAsync(MailTemplateListRequestDto input)
    {
        var query = await _repository.GetQueryableAsync();

        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Name.Contains(input.Filter!) || x.Code.Contains(input.Filter!))
            .WhereIf(input.CategoryId.HasValue, x => x.CategoryId == input.CategoryId!.Value)
            .WhereIf(input.IsEnabled.HasValue, x => x.IsEnabled == input.IsEnabled!.Value);

        var total = query.Count();
        var items = query
            .OrderBy(x => x.Name)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<MailTemplateDto>(
            total,
            ObjectMapper.Map<List<MailTemplate>, List<MailTemplateDto>>(items)
        );
    }

    public async Task<MailTemplateDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<MailTemplate, MailTemplateDto>(entity);
    }

    [Authorize(EmailManagementPermissions.MailTemplates.Create)]
    public async Task<MailTemplateDto> CreateAsync(CreateMailTemplateInput input)
    {
        var entity = new MailTemplate(
            GuidGenerator.Create(),
            input.Code,
            input.Name,
            input.BodyFormat
        );

        entity.CategoryId = input.CategoryId;
        entity.SubjectTemplate = input.SubjectTemplate;
        entity.LanguageCode = input.LanguageCode;
        entity.NeedApproval = input.NeedApproval;
        entity.Description = input.Description;

        if (!string.IsNullOrEmpty(input.BodyContent))
        {
            entity.Versions.Add(new MailTemplateVersion(GuidGenerator.Create(), entity.Id, 1)
            {
                SubjectTemplate = input.SubjectTemplate,
                BodyContent = input.BodyContent,
                IsPublished = true
            });
        }

        await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<MailTemplate, MailTemplateDto>(entity);
    }

    [Authorize(EmailManagementPermissions.MailTemplates.Update)]
    public async Task<MailTemplateDto> UpdateAsync(Guid id, UpdateMailTemplateInput input)
    {
        var entity = await _repository.GetAsync(id);
        entity.Name = input.Name;
        entity.CategoryId = input.CategoryId;
        entity.SubjectTemplate = input.SubjectTemplate;
        entity.NeedApproval = input.NeedApproval;
        entity.Description = input.Description;

        if (!string.IsNullOrEmpty(input.BodyContent))
        {
            entity.IncrementVersion();
            entity.Versions.Add(new MailTemplateVersion(GuidGenerator.Create(), entity.Id, entity.CurrentVersionNo)
            {
                SubjectTemplate = input.SubjectTemplate,
                BodyContent = input.BodyContent,
                ChangeSummary = input.ChangeSummary,
                IsPublished = true
            });
        }

        await _repository.UpdateAsync(entity, autoSave: true);
        return ObjectMapper.Map<MailTemplate, MailTemplateDto>(entity);
    }

    [Authorize(EmailManagementPermissions.MailTemplates.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id, autoSave: true);
    }
}
