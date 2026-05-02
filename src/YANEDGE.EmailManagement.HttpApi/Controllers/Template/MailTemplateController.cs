using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using YANEDGE.EmailManagement.Template;

namespace YANEDGE.EmailManagement.HttpApi.Controllers.Template;

[Area("mail-management")]
[RemoteService(Name = "EmailManagement")]
[Route("api/mail-management/v1/templates")]
public class MailTemplateController : EmailManagementController
{
    private readonly IMailTemplateAppService _templateAppService;

    public MailTemplateController(IMailTemplateAppService templateAppService)
    {
        _templateAppService = templateAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<MailTemplateDto>> GetListAsync([FromQuery] GetTemplateListInput input)
    {
        return _templateAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<MailTemplateDto> GetAsync(Guid id)
    {
        return _templateAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("by-code/{code}")]
    public virtual Task<MailTemplateDto> GetByCodeAsync(string code)
    {
        return _templateAppService.GetByCodeAsync(code);
    }

    [HttpPost]
    public virtual Task<MailTemplateDto> CreateAsync([FromBody] CreateMailTemplateInput input)
    {
        return _templateAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<MailTemplateDto> UpdateAsync(Guid id, [FromBody] UpdateMailTemplateInput input)
    {
        return _templateAppService.UpdateAsync(id, input);
    }

    [HttpPost]
    [Route("{id}/activate")]
    public virtual Task ActivateAsync(Guid id)
    {
        return _templateAppService.ActivateAsync(id);
    }

    [HttpPost]
    [Route("{id}/deactivate")]
    public virtual Task DeactivateAsync(Guid id)
    {
        return _templateAppService.DeactivateAsync(id);
    }

    [HttpPost]
    [Route("{id}/archive")]
    public virtual Task ArchiveAsync(Guid id)
    {
        return _templateAppService.ArchiveAsync(id);
    }

    [HttpPost]
    [Route("{id}/set-default")]
    public virtual Task SetAsDefaultAsync(Guid id)
    {
        return _templateAppService.SetAsDefaultAsync(id);
    }

    [HttpPost]
    [Route("{id}/unset-default")]
    public virtual Task UnsetAsDefaultAsync(Guid id)
    {
        return _templateAppService.UnsetAsDefaultAsync(id);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _templateAppService.DeleteAsync(id);
    }
}
