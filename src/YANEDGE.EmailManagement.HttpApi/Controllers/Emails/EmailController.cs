using System;
using System.Threading.Tasks;
using YANEDGE.EmailManagement.Emails;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace YANEDGE.EmailManagement.Controllers.Emails;

[RemoteService]
[Area("emailManagement")]
[Route("api/email-management/emails")]
public class EmailController : EmailManagementController, IEmailAppService
{
    private readonly IEmailAppService _emailAppService;

    public EmailController(IEmailAppService emailAppService)
    {
        _emailAppService = emailAppService;
    }

    [HttpGet("{id}")]
    public Task<EmailDto> GetAsync(Guid id) => _emailAppService.GetAsync(id);

    [HttpGet]
    public Task<PagedResultDto<EmailDto>> GetListAsync(GetEmailListDto input) => _emailAppService.GetListAsync(input);

    [HttpPost]
    public Task<EmailDto> CreateAsync(CreateEmailDto input) => _emailAppService.CreateAsync(input);

    [HttpPut("{id}")]
    public Task<EmailDto> UpdateAsync(Guid id, UpdateEmailDto input) => _emailAppService.UpdateAsync(id, input);

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id) => _emailAppService.DeleteAsync(id);

    [HttpPost("{id}/queue")]
    public Task<EmailDto> QueueAsync(Guid id, QueueEmailDto input) => _emailAppService.QueueAsync(id, input);

    [HttpPost("{id}/cancel")]
    public Task<EmailDto> CancelAsync(Guid id) => _emailAppService.CancelAsync(id);
}
