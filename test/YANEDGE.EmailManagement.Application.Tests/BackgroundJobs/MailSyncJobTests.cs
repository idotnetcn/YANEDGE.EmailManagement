using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Volo.Abp.Uow;
using Xunit;
using YANEDGE.EmailManagement.Application.BackgroundJobs;
using YANEDGE.EmailManagement.Domain.DomainMailAccount;
using YANEDGE.EmailManagement.Domain.Services;
using DomainDomainMailAccount = YANEDGE.EmailManagement.Domain.DomainMailAccount.DomainMailAccount;

namespace YANEDGE.EmailManagement.Application.Tests.BackgroundJobs;

public class MailSyncJobTests : EmailManagementApplicationTestBase
{
    private readonly MailSyncJob _mailSyncJob;
    private readonly IDomainMailAccountRepository _mailAccountRepository;
    private readonly IMailSyncService _mailSyncService;
    private readonly ILogger<MailSyncJob> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public MailSyncJobTests()
    {
        _mailAccountRepository = Substitute.For<IDomainMailAccountRepository>();
        _mailSyncService = Substitute.For<IMailSyncService>();
        _logger = Substitute.For<ILogger<MailSyncJob>>();
        _unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();

        _mailSyncJob = new MailSyncJob(
            _mailAccountRepository,
            _mailSyncService,
            _logger,
            _unitOfWorkManager);
    }

    [Fact]
    public async Task Should_Execute_Without_Error_When_No_Accounts()
    {
        // Arrange
        _mailAccountRepository.GetSyncEnabledAccountsAsync()
            .Returns(Task.FromResult(new List<DomainDomainMailAccount>()));

        // Act
        await _mailSyncJob.ExecuteAsync();

        // Assert
        await _mailAccountRepository.Received(1).GetSyncEnabledAccountsAsync();
        await _mailSyncService.DidNotReceive().TriggerSyncAsync(Arg.Any<Guid>());
    }

    [Fact]
    public async Task Should_Sync_All_Enabled_Accounts()
    {
        // Arrange
        var account1 = CreateTestAccount();
        var account2 = CreateTestAccount();
        var accounts = new List<DomainMailAccount> { account1, account2 };

        _mailAccountRepository.GetSyncEnabledAccountsAsync()
            .Returns(Task.FromResult(accounts));

        _mailSyncService.TriggerSyncAsync(Arg.Any<Guid>())
            .Returns(Task.CompletedTask);

        // Act
        await _mailSyncJob.ExecuteAsync();

        // Assert
        await _mailAccountRepository.Received(1).GetSyncEnabledAccountsAsync();
        await _mailSyncService.Received(1).TriggerSyncAsync(account1.Id);
        await _mailSyncService.Received(1).TriggerSyncAsync(account2.Id);
    }

    [Fact]
    public async Task Should_Continue_Processing_When_One_Account_Fails()
    {
        // Arrange
        var account1 = CreateTestAccount();
        var account2 = CreateTestAccount();
        var accounts = new List<DomainMailAccount> { account1, account2 };

        _mailAccountRepository.GetSyncEnabledAccountsAsync()
            .Returns(Task.FromResult(accounts));

        _mailSyncService.TriggerSyncAsync(account1.Id)
            .Returns(Task.FromException(new Exception("Sync failed")));

        _mailSyncService.TriggerSyncAsync(account2.Id)
            .Returns(Task.CompletedTask);

        // Act
        await _mailSyncJob.ExecuteAsync();

        // Assert - should still attempt to sync the second account
        await _mailAccountRepository.Received(1).GetSyncEnabledAccountsAsync();
        await _mailSyncService.Received(1).TriggerSyncAsync(account1.Id);
        await _mailSyncService.Received(1).TriggerSyncAsync(account2.Id);
    }

    private DomainMailAccount CreateTestAccount()
    {
        return new DomainMailAccount(
            Guid.NewGuid(),
            $"Test Account {Guid.NewGuid()}",
            $"test{Guid.NewGuid()}@example.com",
            "Test User",
            "Personal",
            "IMAP",
            "imap.example.com",
            993,
            true,
            "SMTP",
            "smtp.example.com",
            587,
            true,
            "testuser",
            "encrypted_password",
            Guid.NewGuid()
        );
    }
}
