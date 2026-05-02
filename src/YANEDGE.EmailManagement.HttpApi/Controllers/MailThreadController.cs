using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using YANEDGE.EmailManagement.Application.Contracts.MailThread;

namespace YANEDGE.EmailManagement.HttpApi.Controllers;

[Route("api/mail-management/v1/threads")]
public class MailThreadController : AbpControllerBase
{
    private readonly IMailThreadAppService _mailThreadAppService;

    public MailThreadController(IMailThreadAppService mailThreadAppService)
    {
        _mailThreadAppService = mailThreadAppService;
    }

    [HttpGet]
    public Task<List<MailThreadDto>> GetListAsync([FromQuery] GetThreadListInput input)
    {
        return _mailThreadAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public Task<MailThreadDto> GetAsync(Guid id)
    {
        return _mailThreadAppService.GetAsync(id);
    }

    [HttpPost("{id}/claim")]
    public Task<MailThreadDto> ClaimAsync(Guid id, [FromBody] ClaimThreadInput input)
    {
        return _mailThreadAppService.ClaimAsync(id, input);
    }

    [HttpPost("{id}/assign")]
    public Task<MailThreadDto> AssignAsync(Guid id, [FromBody] AssignThreadInput input)
    {
        return _mailThreadAppService.AssignAsync(id, input);
    }

    [HttpPost("{id}/archive")]
    public Task<MailThreadDto> ArchiveAsync(Guid id, [FromBody] ArchiveThreadInput input)
    {
        return _mailThreadAppService.ArchiveAsync(id, input);
    }

    [HttpPost("{id}/close")]
    public Task<MailThreadDto> CloseAsync(Guid id)
    {
        return _mailThreadAppService.CloseAsync(id);
    }

    [HttpPost("{id}/reopen")]
    public Task<MailThreadDto> ReopenAsync(Guid id)
    {
        return _mailThreadAppService.ReopenAsync(id);
    }
}
