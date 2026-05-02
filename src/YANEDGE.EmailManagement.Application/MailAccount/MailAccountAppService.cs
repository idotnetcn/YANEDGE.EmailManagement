using Volo.Abp.Application.Services;
using YANEDGE.EmailManagement.Application.Contracts.MailAccount;
using YANEDGE.EmailManagement.Domain.MailAccount;
using YANEDGE.EmailManagement.Domain.Services;

namespace YANEDGE.EmailManagement.Application.MailAccount;

/// <summary>
/// 邮箱账号应用服务
/// </summary>
public class MailAccountAppService : ApplicationService, IMailAccountAppService
{
    private readonly IMailAccountRepository _mailAccountRepository;
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    private readonly IMailConnectionTestService _connectionTestService;
    private readonly IMailSyncService _mailSyncService;

    public MailAccountAppService(
        IMailAccountRepository mailAccountRepository,
        IPasswordEncryptionService passwordEncryptionService,
        IMailConnectionTestService connectionTestService,
        IMailSyncService mailSyncService)
    {
        _mailAccountRepository = mailAccountRepository;
        _passwordEncryptionService = passwordEncryptionService;
        _connectionTestService = connectionTestService;
        _mailSyncService = mailSyncService;
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
        // Encrypt password before saving
        var encryptedPassword = _passwordEncryptionService.Encrypt(input.Password);

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
        var account = await _mailAccountRepository.GetAsync(id);

        var result = await _connectionTestService.TestConnectionAsync(account);

        return new TestConnectionResult
        {
            IncomingSuccess = result.IncomingSuccess,
            OutgoingSuccess = result.OutgoingSuccess,
            Detail = result.Detail ?? (result.IsSuccess ? "Connection test passed" : "Connection test failed")
        };
    }

    public async Task<SyncJobResult> TriggerSyncAsync(Guid id)
    {
        var result = await _mailSyncService.TriggerSyncAsync(id);

        return new SyncJobResult
        {
            JobId = result.JobId,
            Accepted = result.Accepted,
            Message = result.Message ?? "Sync job triggered successfully"
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
