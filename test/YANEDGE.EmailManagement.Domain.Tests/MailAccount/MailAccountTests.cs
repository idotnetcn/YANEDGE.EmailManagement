using Shouldly;
using Xunit;
using YANEDGE.EmailManagement.Domain.MailAccount;
using YANEDGE.EmailManagement.Enums;

namespace YANEDGE.EmailManagement.Domain.Tests.MailAccount;

public class MailAccountTests : EmailManagementDomainTestBase
{
    [Fact]
    public void Should_Create_MailAccount_With_Valid_Data()
    {
        // Arrange
        var id = Guid.NewGuid();
        var accountName = "Test Account";
        var emailAddress = "test@example.com";
        var displayName = "Test User";
        var accountType = MailAccountType.Personal;
        var incomingProtocol = MailProtocol.IMAP;
        var incomingHost = "imap.example.com";
        var incomingPort = 993;
        var incomingSslEnabled = true;
        var outgoingProtocol = MailProtocol.IMAP;
        var outgoingHost = "smtp.example.com";
        var outgoingPort = 465;
        var outgoingSslEnabled = true;
        var username = "testuser";
        var encryptedPassword = "encrypted_password";
        var ownerOrgId = Guid.NewGuid();

        // Act
        var mailAccount = new Domain.MailAccount.MailAccount(
            id,
            accountName,
            emailAddress,
            displayName,
            accountType,
            incomingProtocol,
            incomingHost,
            incomingPort,
            incomingSslEnabled,
            outgoingProtocol,
            outgoingHost,
            outgoingPort,
            outgoingSslEnabled,
            username,
            encryptedPassword,
            ownerOrgId
        );

        // Assert
        mailAccount.ShouldNotBeNull();
        mailAccount.Id.ShouldBe(id);
        mailAccount.AccountName.ShouldBe(accountName);
        mailAccount.EmailAddress.ShouldBe(emailAddress);
        mailAccount.DisplayName.ShouldBe(displayName);
        mailAccount.AccountType.ShouldBe(accountType);
        mailAccount.IncomingProtocol.ShouldBe(incomingProtocol);
        mailAccount.IncomingHost.ShouldBe(incomingHost);
        mailAccount.IncomingPort.ShouldBe(incomingPort);
        mailAccount.IncomingSslEnabled.ShouldBe(incomingSslEnabled);
        mailAccount.OutgoingProtocol.ShouldBe(outgoingProtocol);
        mailAccount.OutgoingHost.ShouldBe(outgoingHost);
        mailAccount.OutgoingPort.ShouldBe(outgoingPort);
        mailAccount.OutgoingSslEnabled.ShouldBe(outgoingSslEnabled);
        mailAccount.Username.ShouldBe(username);
        mailAccount.EncryptedPassword.ShouldBe(encryptedPassword);
        mailAccount.OwnerOrgId.ShouldBe(ownerOrgId);
        mailAccount.SyncEnabled.ShouldBeTrue();
        mailAccount.SendEnabled.ShouldBeTrue();
        mailAccount.HealthStatus.ShouldBe(1);
    }

    [Fact]
    public void Should_Enable_Sync()
    {
        // Arrange
        var mailAccount = CreateTestMailAccount();
        mailAccount.DisableSync();

        // Act
        mailAccount.EnableSync();

        // Assert
        mailAccount.SyncEnabled.ShouldBeTrue();
    }

    [Fact]
    public void Should_Disable_Sync()
    {
        // Arrange
        var mailAccount = CreateTestMailAccount();

        // Act
        mailAccount.DisableSync();

        // Assert
        mailAccount.SyncEnabled.ShouldBeFalse();
    }

    [Fact]
    public void Should_Enable_Send()
    {
        // Arrange
        var mailAccount = CreateTestMailAccount();
        mailAccount.DisableSend();

        // Act
        mailAccount.EnableSend();

        // Assert
        mailAccount.SendEnabled.ShouldBeTrue();
    }

    [Fact]
    public void Should_Disable_Send()
    {
        // Arrange
        var mailAccount = CreateTestMailAccount();

        // Act
        mailAccount.DisableSend();

        // Assert
        mailAccount.SendEnabled.ShouldBeFalse();
    }

    [Fact]
    public void Should_Update_Last_Sync_Time()
    {
        // Arrange
        var mailAccount = CreateTestMailAccount();
        var syncTime = DateTime.UtcNow;

        // Act
        mailAccount.UpdateLastSyncTime(syncTime);

        // Assert
        mailAccount.LastSyncAt.ShouldBe(syncTime);
    }

    [Fact]
    public void Should_Update_Last_Send_Time()
    {
        // Arrange
        var mailAccount = CreateTestMailAccount();
        var sendTime = DateTime.UtcNow;

        // Act
        mailAccount.UpdateLastSendTime(sendTime);

        // Assert
        mailAccount.LastSendAt.ShouldBe(sendTime);
    }

    [Fact]
    public void Should_Update_Health_Status()
    {
        // Arrange
        var mailAccount = CreateTestMailAccount();
        var newHealthStatus = 0; // Unhealthy

        // Act
        mailAccount.UpdateHealthStatus(newHealthStatus);

        // Assert
        mailAccount.HealthStatus.ShouldBe(newHealthStatus);
    }

    private Domain.MailAccount.MailAccount CreateTestMailAccount()
    {
        return new Domain.MailAccount.MailAccount(
            Guid.NewGuid(),
            "Test Account",
            "test@example.com",
            "Test User",
            MailAccountType.Personal,
            MailProtocol.IMAP,
            "imap.example.com",
            993,
            true,
            MailProtocol.IMAP,
            "smtp.example.com",
            465,
            true,
            "testuser",
            "encrypted_password"
        );
    }
}
