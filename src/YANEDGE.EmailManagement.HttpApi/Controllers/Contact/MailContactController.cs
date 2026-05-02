using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using YANEDGE.EmailManagement.Contact;

namespace YANEDGE.EmailManagement.HttpApi.Controllers.Contact;

[Area("mail-management")]
[RemoteService(Name = "EmailManagement")]
[Route("api/mail-management/v1/contacts")]
public class MailContactController : AbpControllerBase
{
    private readonly IMailContactAppService _contactAppService;

    public MailContactController(IMailContactAppService contactAppService)
    {
        _contactAppService = contactAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<MailContactDto>> GetListAsync([FromQuery] GetContactListInput input)
    {
        return _contactAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<MailContactDto> GetAsync(Guid id)
    {
        return _contactAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("by-email/{emailAddress}")]
    public virtual Task<MailContactDto> GetByEmailAsync(string emailAddress)
    {
        return _contactAppService.GetByEmailAsync(emailAddress);
    }

    [HttpPost]
    public virtual Task<MailContactDto> CreateAsync([FromBody] CreateMailContactInput input)
    {
        return _contactAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<MailContactDto> UpdateAsync(Guid id, [FromBody] UpdateMailContactInput input)
    {
        return _contactAppService.UpdateAsync(id, input);
    }

    [HttpPost]
    [Route("{id}/verify")]
    public virtual Task VerifyAsync(Guid id)
    {
        return _contactAppService.VerifyAsync(id);
    }

    [HttpPost]
    [Route("{id}/activate")]
    public virtual Task ActivateAsync(Guid id)
    {
        return _contactAppService.ActivateAsync(id);
    }

    [HttpPost]
    [Route("{id}/deactivate")]
    public virtual Task DeactivateAsync(Guid id)
    {
        return _contactAppService.DeactivateAsync(id);
    }

    [HttpPost]
    [Route("{id}/link-customer")]
    public virtual Task LinkToCustomerAsync(Guid id, [FromBody] LinkToCustomerRequest request)
    {
        return _contactAppService.LinkToCustomerAsync(id, request.CustomerId);
    }

    [HttpPost]
    [Route("{id}/link-supplier")]
    public virtual Task LinkToSupplierAsync(Guid id, [FromBody] LinkToSupplierRequest request)
    {
        return _contactAppService.LinkToSupplierAsync(id, request.SupplierId);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _contactAppService.DeleteAsync(id);
    }
}

public class LinkToCustomerRequest
{
    public string CustomerId { get; set; } = null!;
}

public class LinkToSupplierRequest
{
    public string SupplierId { get; set; } = null!;
}
