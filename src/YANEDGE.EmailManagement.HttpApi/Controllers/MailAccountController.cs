using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using YANEDGE.EmailManagement.Application.Contracts.MailAccount;

namespace YANEDGE.EmailManagement.HttpApi.Controllers;

[Route("api/mail-management/v1/mail-accounts")]
public class MailAccountController : AbpControllerBase
{
    private readonly IMailAccountAppService _mailAccountAppService;

    public MailAccountController(IMailAccountAppService mailAccountAppService)
    {
        _mailAccountAppService = mailAccountAppService;
    }

    [HttpGet]
    public Task<List<MailAccountDto>> GetListAsync()
    {
        return _mailAccountAppService.GetListAsync();
    }

    [HttpGet("{id}")]
    public Task<MailAccountDto> GetAsync(Guid id)
    {
        return _mailAccountAppService.GetAsync(id);
    }

    [HttpPost]
    public Task<MailAccountDto> CreateAsync(CreateMailAccountInput input)
    {
        return _mailAccountAppService.CreateAsync(input);
    }

    [HttpPost("{id}/sync-toggle")]
    public Task<MailAccountDto> ToggleSyncAsync(Guid id, [FromBody] ToggleSyncRequest request)
    {
        return _mailAccountAppService.ToggleSyncAsync(id, request.Enabled);
    }

    [HttpPost("{id}/test-connection")]
    public Task<TestConnectionResult> TestConnectionAsync(Guid id)
    {
        return _mailAccountAppService.TestConnectionAsync(id);
    }

    [HttpPost("{id}/sync")]
    public Task<SyncJobResult> TriggerSyncAsync(Guid id)
    {
        return _mailAccountAppService.TriggerSyncAsync(id);
    }
}

public class ToggleSyncRequest
{
    public bool Enabled { get; set; }
}
