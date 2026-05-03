using System;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using Volo.Abp.DependencyInjection;

namespace YANEDGE.EmailManagement.Domain.Services.Implementations;

/// <summary>
/// 邮箱连接测试服务实现
/// </summary>
public class MailConnectionTestService : IMailConnectionTestService, ITransientDependency
{
    private readonly IPasswordEncryptionService _passwordEncryptionService;

    public MailConnectionTestService(IPasswordEncryptionService passwordEncryptionService)
    {
        _passwordEncryptionService = passwordEncryptionService;
    }

    public async Task<MailConnectionTestResult> TestConnectionAsync(MailAccount.MailAccount account)
    {
        var result = new MailConnectionTestResult();

        // 解密密码
        string plainPassword;
        try
        {
            plainPassword = _passwordEncryptionService.Decrypt(account.EncryptedPassword);
        }
        catch (Exception ex)
        {
            result.IncomingError = "密码解密失败";
            result.OutgoingError = "密码解密失败";
            result.Detail = ex.Message;
            return result;
        }

        // 测试接收服务器 (IMAP)
        await TestIncomingServerAsync(account, plainPassword, result);

        // 测试发送服务器 (SMTP)
        await TestOutgoingServerAsync(account, plainPassword, result);

        return result;
    }

    private async Task TestIncomingServerAsync(
        MailAccount.MailAccount account,
        string plainPassword,
        MailConnectionTestResult result)
    {
        using var client = new ImapClient();
        try
        {
            var secureSocketOptions = account.IncomingSslEnabled
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;

            await client.ConnectAsync(account.IncomingHost, account.IncomingPort, secureSocketOptions);
            await client.AuthenticateAsync(account.Username, plainPassword);

            result.IncomingSuccess = true;
            result.IncomingError = null;

            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            result.IncomingSuccess = false;
            result.IncomingError = $"IMAP连接失败: {ex.Message}";
        }
    }

    private async Task TestOutgoingServerAsync(
        MailAccount.MailAccount account,
        string plainPassword,
        MailConnectionTestResult result)
    {
        using var client = new SmtpClient();
        try
        {
            var secureSocketOptions = account.OutgoingSslEnabled
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;

            await client.ConnectAsync(account.OutgoingHost, account.OutgoingPort, secureSocketOptions);

            if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
            {
                await client.AuthenticateAsync(account.Username, plainPassword);
            }

            result.OutgoingSuccess = true;
            result.OutgoingError = null;

            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            result.OutgoingSuccess = false;
            result.OutgoingError = $"SMTP连接失败: {ex.Message}";
        }
    }
}
