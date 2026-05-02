using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using YANEDGE.EmailManagement.Label;

namespace YANEDGE.EmailManagement.HttpApi.Controllers.Label;

[Area("mail-management")]
[RemoteService(Name = "EmailManagement")]
[Route("api/mail-management/v1/labels")]
public class MailLabelController : AbpControllerBase
{
    private readonly IMailLabelAppService _labelAppService;

    public MailLabelController(IMailLabelAppService labelAppService)
    {
        _labelAppService = labelAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<MailLabelDto>> GetListAsync([FromQuery] GetLabelListInput input)
    {
        return _labelAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<MailLabelDto> GetAsync(Guid id)
    {
        return _labelAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<MailLabelDto> CreateAsync([FromBody] CreateMailLabelInput input)
    {
        return _labelAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<MailLabelDto> UpdateAsync(Guid id, [FromBody] UpdateMailLabelInput input)
    {
        return _labelAppService.UpdateAsync(id, input);
    }

    [HttpPost]
    [Route("{id}/activate")]
    public virtual Task ActivateAsync(Guid id)
    {
        return _labelAppService.ActivateAsync(id);
    }

    [HttpPost]
    [Route("{id}/deactivate")]
    public virtual Task DeactivateAsync(Guid id)
    {
        return _labelAppService.DeactivateAsync(id);
    }

    [HttpPost]
    [Route("{id}/sort-order")]
    public virtual Task SetSortOrderAsync(Guid id, [FromBody] SetSortOrderRequest request)
    {
        return _labelAppService.SetSortOrderAsync(id, request.SortOrder);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _labelAppService.DeleteAsync(id);
    }
}

public class SetSortOrderRequest
{
    public int SortOrder { get; set; }
}
