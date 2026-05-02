using Volo.Abp.Application.Services;
using YANEDGE.EmailManagement.Application.Contracts.MailAccount;
using YANEDGE.EmailManagement.Domain.MailAccount;

namespace YANEDGE.EmailManagement.Application.MailAccount;

/// <summary>
/// 邮箱账号应用服务
/// </summary>
public class MailAccountAppService : ApplicationService, IMailAccountAppService
{
    private readonly IMailAccountRepository _mailAccountRepository;

    public MailAccountAppService(IMailAccountRepository mailAccountRepository)
    {
        _mailAccountRepository = mailAccountRepository;
    }

    public async Task<List<MailAccountDto>> GetListAsync()
    {
        var accounts = await _mailAccountRepository.GetListAsync();

        return accounts.Select(MapToDto).ToList();
    }

    public async Task<MailAccountDto> GetAsync(Guid id)
    {
        var account = await _mailAccountRepository.GetAsync(id);

        return MapToDto(account);
    }

    public async Task<MailAccountDto> CreateAsync(CreateMailAccountInput input)
    {
        // TODO: Encrypt password before saving
        var encryptedPassword = input.Password; // Should use encryption service

        var account = new Domain.MailAccount.MailAccount(
            GuidGenerator.Create(),
            input.AccountName,
            input.EmailAddress,
            input.DisplayName,
            input.AccountType,
            input.IncomingProtocol,
            input.IncomingHost,
            input.IncomingPort,
            input.IncomingSslEnabled,
            input.OutgoingProtocol,
            input.OutgoingHost,
            input.OutgoingPort,
            input.OutgoingSslEnabled,
            input.Username,
            encryptedPassword,
            input.OwnerOrgId
        );

        await _mailAccountRepository.InsertAsync(account);

        return MapToDto(account);
    }

    public async Task<MailAccountDto> ToggleSyncAsync(Guid id, bool enabled)
    {
        var account = await _mailAccountRepository.GetAsync(id);

        if (enabled)
        {
            account.EnableSync();
        }
        else
        {
            account.DisableSync();
        }

        await _mailAccountRepository.UpdateAsync(account);

        return MapToDto(account);
    }

    public async Task<TestConnectionResult> TestConnectionAsync(Guid id)
    {
        // TODO: Implement actual connection test
        await Task.CompletedTask;

        return new TestConnectionResult
        {
            IncomingSuccess = true,
            OutgoingSuccess = true,
            Detail = "Connection test passed (mock implementation)"
        };
    }

    public async Task<SyncJobResult> TriggerSyncAsync(Guid id)
    {
        // TODO: Implement actual sync trigger
        await Task.CompletedTask;

        return new SyncJobResult
        {
            JobId = Guid.NewGuid(),
            Accepted = true,
            Message = "Sync job triggered (mock implementation)"
        };
    }

    private MailAccountDto MapToDto(Domain.MailAccount.MailAccount account)
    {
        return new MailAccountDto
        {
            Id = account.Id,
            AccountName = account.AccountName,
            EmailAddress = account.EmailAddress,
            DisplayName = account.DisplayName,
            AccountType = account.AccountType,
            AccountTypeName = account.AccountType.ToString(),
            SyncEnabled = account.SyncEnabled,
            SendEnabled = account.SendEnabled,
            HealthStatus = account.HealthStatus,
            HealthStatusName = account.HealthStatus == 1 ? "Healthy" : "Unhealthy",
            LastSyncAt = account.LastSyncAt,
            LastSendAt = account.LastSendAt
        };
    }
}
