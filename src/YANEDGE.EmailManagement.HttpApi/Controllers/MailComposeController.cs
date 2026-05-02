using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using YANEDGE.EmailManagement.Application.Contracts.MailCompose;

namespace YANEDGE.EmailManagement.HttpApi.Controllers;

[Route("api/mail-management/v1/send-tasks")]
public class MailComposeController : AbpControllerBase
{
    private readonly IMailComposeAppService _mailComposeAppService;

    public MailComposeController(IMailComposeAppService mailComposeAppService)
    {
        _mailComposeAppService = mailComposeAppService;
    }

    [HttpGet]
    public Task<List<MailSendTaskDto>> GetListAsync([FromQuery] GetSendTaskListInput input)
    {
        return _mailComposeAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public Task<MailSendTaskDto> GetAsync(Guid id)
    {
        return _mailComposeAppService.GetAsync(id);
    }

    [HttpPost]
    public Task<MailSendTaskDto> CreateAsync([FromBody] CreateSendTaskInput input)
    {
        return _mailComposeAppService.CreateAsync(input);
    }

    [HttpPost("{id}/submit-approval")]
    public Task<MailSendTaskDto> SubmitApprovalAsync(Guid id)
    {
        return _mailComposeAppService.SubmitApprovalAsync(id);
    }

    [HttpPost("{id}/send")]
    public Task<MailSendTaskDto> SendAsync(Guid id)
    {
        return _mailComposeAppService.SendAsync(id);
    }

    [HttpPost("{id}/cancel")]
    public Task<MailSendTaskDto> CancelAsync(Guid id)
    {
        return _mailComposeAppService.CancelAsync(id);
    }

    [HttpPost("{id}/retry")]
    public Task<MailSendTaskDto> RetryAsync(Guid id)
    {
        return _mailComposeAppService.RetryAsync(id);
    }
}
