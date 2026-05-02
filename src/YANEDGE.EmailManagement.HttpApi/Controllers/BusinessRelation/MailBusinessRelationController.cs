using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using YANEDGE.EmailManagement.BusinessRelation;

namespace YANEDGE.EmailManagement.HttpApi.Controllers.BusinessRelation;

[Area("mail-management")]
[RemoteService(Name = "EmailManagement")]
[Route("api/mail-management/v1/business-relations")]
public class MailBusinessRelationController : AbpControllerBase
{
    private readonly IMailBusinessRelationAppService _businessRelationAppService;

    public MailBusinessRelationController(IMailBusinessRelationAppService businessRelationAppService)
    {
        _businessRelationAppService = businessRelationAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<MailBusinessRelationDto>> GetListAsync([FromQuery] GetBusinessRelationListInput input)
    {
        return _businessRelationAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<MailBusinessRelationDto> GetAsync(Guid id)
    {
        return _businessRelationAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<MailBusinessRelationDto> CreateAsync([FromBody] CreateMailBusinessRelationInput input)
    {
        return _businessRelationAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<MailBusinessRelationDto> UpdateAsync(Guid id, [FromBody] UpdateMailBusinessRelationInput input)
    {
        return _businessRelationAppService.UpdateAsync(id, input);
    }

    [HttpPost]
    [Route("{id}/set-primary")]
    public virtual Task SetAsPrimaryAsync(Guid id)
    {
        return _businessRelationAppService.SetAsPrimaryAsync(id);
    }

    [HttpPost]
    [Route("{id}/unset-primary")]
    public virtual Task UnsetAsPrimaryAsync(Guid id)
    {
        return _businessRelationAppService.UnsetAsPrimaryAsync(id);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _businessRelationAppService.DeleteAsync(id);
    }
}
