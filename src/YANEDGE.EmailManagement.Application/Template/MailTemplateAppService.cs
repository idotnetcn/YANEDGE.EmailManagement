using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using YANEDGE.EmailManagement.Template;
using YANEDGE.EmailManagement.Permissions;
using YANEDGE.EmailManagement.Enums;
using Microsoft.AspNetCore.Authorization;

namespace YANEDGE.EmailManagement.Application.Template;

[Authorize(EmailManagementPermissions.Templates.Default)]
public class MailTemplateAppService : ApplicationService, IMailTemplateAppService
{
    private readonly IRepository<Domain.Template.MailTemplate, Guid> _templateRepository;
    private readonly Domain.Template.IMailTemplateRepository _mailTemplateRepository;

    public MailTemplateAppService(
        IRepository<Domain.Template.MailTemplate, Guid> templateRepository,
        Domain.Template.IMailTemplateRepository mailTemplateRepository)
    {
        _templateRepository = templateRepository;
        _mailTemplateRepository = mailTemplateRepository;
    }

    public async Task<PagedResultDto<MailTemplateDto>> GetListAsync(GetTemplateListInput input)
    {
        var query = await _templateRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.Category))
        {
            query = query.Where(x => x.Category == input.Category);
        }

        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            query = query.Where(x => x.Name.Contains(input.Keyword) || x.Code.Contains(input.Keyword));
        }

        if (input.Status.HasValue)
        {
            query = query.Where(x => x.Status == input.Status.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query.OrderBy(x => x.SortOrder).ThenBy(x => x.Name)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<MailTemplateDto>(
            totalCount,
            ObjectMapper.Map<System.Collections.Generic.List<Domain.Template.MailTemplate>, System.Collections.Generic.List<MailTemplateDto>>(items)
        );
    }

    public async Task<MailTemplateDto> GetAsync(Guid id)
    {
        var template = await _templateRepository.GetAsync(id);
        return ObjectMapper.Map<Domain.Template.MailTemplate, MailTemplateDto>(template);
    }

    public async Task<MailTemplateDto> GetByCodeAsync(string code)
    {
        var template = await _mailTemplateRepository.FindByCodeAsync(code);
        if (template == null)
        {
            throw new Volo.Abp.BusinessException("Template:NotFound")
                .WithData("Code", code);
        }
        return ObjectMapper.Map<Domain.Template.MailTemplate, MailTemplateDto>(template);
    }

    [Authorize(EmailManagementPermissions.Templates.Create)]
    public async Task<MailTemplateDto> CreateAsync(CreateMailTemplateInput input)
    {
        var existingTemplate = await _mailTemplateRepository.FindByCodeAsync(input.Code);
        if (existingTemplate != null)
        {
            throw new Volo.Abp.BusinessException("Template:CodeAlreadyExists")
                .WithData("Code", input.Code);
        }

        var template = new Domain.Template.MailTemplate(
            GuidGenerator.Create(),
            input.Code,
            input.Name,
            input.SubjectTemplate,
            input.BodyTemplate,
            input.Language,
            input.Category,
            input.RequiresApproval,
            input.Description
        );

        if (input.IsDefault)
        {
            template.SetAsDefault();
        }

        await _templateRepository.InsertAsync(template);

        return ObjectMapper.Map<Domain.Template.MailTemplate, MailTemplateDto>(template);
    }

    [Authorize(EmailManagementPermissions.Templates.Update)]
    public async Task<MailTemplateDto> UpdateAsync(Guid id, UpdateMailTemplateInput input)
    {
        var template = await _templateRepository.GetAsync(id);

        template.Update(
            input.Name,
            input.SubjectTemplate,
            input.BodyTemplate,
            input.Category,
            input.PlainTextTemplate,
            input.Description
        );

        await _templateRepository.UpdateAsync(template);

        return ObjectMapper.Map<Domain.Template.MailTemplate, MailTemplateDto>(template);
    }

    [Authorize(EmailManagementPermissions.Templates.Update)]
    public async Task ActivateAsync(Guid id)
    {
        var template = await _templateRepository.GetAsync(id);
        template.Activate();
        await _templateRepository.UpdateAsync(template);
    }

    [Authorize(EmailManagementPermissions.Templates.Update)]
    public async Task DeactivateAsync(Guid id)
    {
        var template = await _templateRepository.GetAsync(id);
        template.Deactivate();
        await _templateRepository.UpdateAsync(template);
    }

    [Authorize(EmailManagementPermissions.Templates.Update)]
    public async Task ArchiveAsync(Guid id)
    {
        var template = await _templateRepository.GetAsync(id);
        template.Archive();
        await _templateRepository.UpdateAsync(template);
    }

    [Authorize(EmailManagementPermissions.Templates.Update)]
    public async Task SetAsDefaultAsync(Guid id)
    {
        var template = await _templateRepository.GetAsync(id);
        template.SetAsDefault();
        await _templateRepository.UpdateAsync(template);
    }

    [Authorize(EmailManagementPermissions.Templates.Update)]
    public async Task UnsetAsDefaultAsync(Guid id)
    {
        var template = await _templateRepository.GetAsync(id);
        template.UnsetAsDefault();
        await _templateRepository.UpdateAsync(template);
    }

    [Authorize(EmailManagementPermissions.Templates.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _templateRepository.DeleteAsync(id);
    }
}
