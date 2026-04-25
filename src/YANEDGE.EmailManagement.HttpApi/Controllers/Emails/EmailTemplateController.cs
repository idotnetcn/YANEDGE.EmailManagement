using System;
using System.Threading.Tasks;
using YANEDGE.EmailManagement.Emails;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Controllers.Emails;

[RemoteService]
[Area("emailManagement")]
[Route("api/email-management/email-templates")]
public class EmailTemplateController : EmailManagementController, IEmailTemplateAppService
{
    private readonly IEmailTemplateAppService _templateAppService;

    public EmailTemplateController(IEmailTemplateAppService templateAppService)
    {
        _templateAppService = templateAppService;
    }

    [HttpGet("{id}")]
    public Task<EmailTemplateDto> GetAsync(Guid id) => _templateAppService.GetAsync(id);

    [HttpGet]
    public Task<PagedResultDto<EmailTemplateDto>> GetListAsync(GetEmailTemplateListDto input) => _templateAppService.GetListAsync(input);

    [HttpPost]
    public Task<EmailTemplateDto> CreateAsync(CreateEmailTemplateDto input) => _templateAppService.CreateAsync(input);

    [HttpPut("{id}")]
    public Task<EmailTemplateDto> UpdateAsync(Guid id, UpdateEmailTemplateDto input) => _templateAppService.UpdateAsync(id, input);

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id) => _templateAppService.DeleteAsync(id);

    [HttpPost("{id}/activate")]
    public Task<EmailTemplateDto> ActivateAsync(Guid id) => _templateAppService.ActivateAsync(id);

    [HttpPost("{id}/deactivate")]
    public Task<EmailTemplateDto> DeactivateAsync(Guid id) => _templateAppService.DeactivateAsync(id);
}
