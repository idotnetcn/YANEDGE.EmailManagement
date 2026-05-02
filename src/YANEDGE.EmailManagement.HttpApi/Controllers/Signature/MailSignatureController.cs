using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using YANEDGE.EmailManagement.Signature;

namespace YANEDGE.EmailManagement.HttpApi.Controllers.Signature;

[Area("mail-management")]
[RemoteService(Name = "EmailManagement")]
[Route("api/mail-management/v1/signatures")]
public class MailSignatureController : EmailManagementController
{
    private readonly IMailSignatureAppService _signatureAppService;

    public MailSignatureController(IMailSignatureAppService signatureAppService)
    {
        _signatureAppService = signatureAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<MailSignatureDto>> GetListAsync([FromQuery] GetSignatureListInput input)
    {
        return _signatureAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<MailSignatureDto> GetAsync(Guid id)
    {
        return _signatureAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("default/user/{userId}")]
    public virtual Task<MailSignatureDto?> GetDefaultByUserIdAsync(Guid userId)
    {
        return _signatureAppService.GetDefaultByUserIdAsync(userId);
    }

    [HttpPost]
    public virtual Task<MailSignatureDto> CreateAsync([FromBody] CreateMailSignatureInput input)
    {
        return _signatureAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<MailSignatureDto> UpdateAsync(Guid id, [FromBody] UpdateMailSignatureInput input)
    {
        return _signatureAppService.UpdateAsync(id, input);
    }

    [HttpPost]
    [Route("{id}/activate")]
    public virtual Task ActivateAsync(Guid id)
    {
        return _signatureAppService.ActivateAsync(id);
    }

    [HttpPost]
    [Route("{id}/deactivate")]
    public virtual Task DeactivateAsync(Guid id)
    {
        return _signatureAppService.DeactivateAsync(id);
    }

    [HttpPost]
    [Route("{id}/set-default")]
    public virtual Task SetAsDefaultAsync(Guid id)
    {
        return _signatureAppService.SetAsDefaultAsync(id);
    }

    [HttpPost]
    [Route("{id}/unset-default")]
    public virtual Task UnsetAsDefaultAsync(Guid id)
    {
        return _signatureAppService.UnsetAsDefaultAsync(id);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _signatureAppService.DeleteAsync(id);
    }
}
