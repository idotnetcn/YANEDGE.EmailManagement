using System;
using System.Threading.Tasks;
using YANEDGE.EmailManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YANEDGE.EmailManagement.Emails;

[Authorize(EmailManagementPermissions.Templates.Default)]
public class EmailTemplateAppService : ApplicationService, IEmailTemplateAppService
{
    private readonly IEmailTemplateRepository _templateRepository;

    public EmailTemplateAppService(IEmailTemplateRepository templateRepository)
    {
        _templateRepository = templateRepository;
    }

    public async Task<EmailTemplateDto> GetAsync(Guid id)
    {
        var template = await _templateRepository.GetAsync(id);
        return ObjectMapper.Map<EmailTemplate, EmailTemplateDto>(template);
    }

    public async Task<PagedResultDto<EmailTemplateDto>> GetListAsync(GetEmailTemplateListDto input)
    {
        var templates = await _templateRepository.GetListAsync(
            input.IsActive,
            input.Filter,
            input.MaxResultCount,
            input.SkipCount);

        return new PagedResultDto<EmailTemplateDto>(
            templates.Count,
            ObjectMapper.Map<System.Collections.Generic.List<EmailTemplate>, System.Collections.Generic.List<EmailTemplateDto>>(templates));
    }

    [Authorize(EmailManagementPermissions.Templates.Create)]
    public async Task<EmailTemplateDto> CreateAsync(CreateEmailTemplateDto input)
    {
        var template = new EmailTemplate(
            GuidGenerator.Create(),
            input.Name,
            input.Subject,
            input.Body,
            input.IsBodyHtml,
            input.Description);
        await _templateRepository.InsertAsync(template);
        return ObjectMapper.Map<EmailTemplate, EmailTemplateDto>(template);
    }

    [Authorize(EmailManagementPermissions.Templates.Edit)]
    public async Task<EmailTemplateDto> UpdateAsync(Guid id, UpdateEmailTemplateDto input)
    {
        var template = await _templateRepository.GetAsync(id);
        template.Update(input.Name, input.Subject, input.Body, input.IsBodyHtml, input.Description);
        await _templateRepository.UpdateAsync(template);
        return ObjectMapper.Map<EmailTemplate, EmailTemplateDto>(template);
    }

    [Authorize(EmailManagementPermissions.Templates.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _templateRepository.DeleteAsync(id);
    }

    [Authorize(EmailManagementPermissions.Templates.Edit)]
    public async Task<EmailTemplateDto> ActivateAsync(Guid id)
    {
        var template = await _templateRepository.GetAsync(id);
        template.Activate();
        await _templateRepository.UpdateAsync(template);
        return ObjectMapper.Map<EmailTemplate, EmailTemplateDto>(template);
    }

    [Authorize(EmailManagementPermissions.Templates.Edit)]
    public async Task<EmailTemplateDto> DeactivateAsync(Guid id)
    {
        var template = await _templateRepository.GetAsync(id);
        template.Deactivate();
        await _templateRepository.UpdateAsync(template);
        return ObjectMapper.Map<EmailTemplate, EmailTemplateDto>(template);
    }
}
