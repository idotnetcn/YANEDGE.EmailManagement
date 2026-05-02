using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using YANEDGE.EmailManagement.Rule;

namespace YANEDGE.EmailManagement.HttpApi.Controllers.Rule;

[Area("mail-management")]
[RemoteService(Name = "EmailManagement")]
[Route("api/mail-management/v1/rules")]
public class MailRuleController : AbpControllerBase
{
    private readonly IMailRuleAppService _ruleAppService;

    public MailRuleController(IMailRuleAppService ruleAppService)
    {
        _ruleAppService = ruleAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<MailRuleDto>> GetListAsync([FromQuery] GetRuleListInput input)
    {
        return _ruleAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<MailRuleDto> GetAsync(Guid id)
    {
        return _ruleAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<MailRuleDto> CreateAsync([FromBody] CreateMailRuleInput input)
    {
        return _ruleAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<MailRuleDto> UpdateAsync(Guid id, [FromBody] UpdateMailRuleInput input)
    {
        return _ruleAppService.UpdateAsync(id, input);
    }

    [HttpPost]
    [Route("{id}/activate")]
    public virtual Task ActivateAsync(Guid id)
    {
        return _ruleAppService.ActivateAsync(id);
    }

    [HttpPost]
    [Route("{id}/deactivate")]
    public virtual Task DeactivateAsync(Guid id)
    {
        return _ruleAppService.DeactivateAsync(id);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _ruleAppService.DeleteAsync(id);
    }
}
