using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using YANEDGE.EmailManagement.Domain.Services;
using YANEDGE.EmailManagement.Domain.MailAccount;

namespace YANEDGE.EmailManagement.Services.Implementation;

/// <summary>
/// 邮件连接测试服务实现
/// </summary>
public class MailConnectionTestService : IMailConnectionTestService, ITransientDependency
{
    private readonly IMailProtocolAdapter _protocolAdapter;

    public MailConnectionTestService(IMailProtocolAdapter protocolAdapter)
    {
        _protocolAdapter = protocolAdapter;
    }

    public async Task<MailConnectionTestResult> TestConnectionAsync(MailAccount account)
    {
        var result = new MailConnectionTestResult();

        try
        {
            // 使用协议适配器进行真实的连接测试
            var success = await _protocolAdapter.TestConnectionAsync(account);

            if (success)
            {
                result.IncomingSuccess = true;
                result.OutgoingSuccess = true;
                result.Detail = "Connection test completed successfully";
            }
            else
            {
                result.IncomingSuccess = false;
                result.OutgoingSuccess = false;
                result.IncomingError = "Connection test failed";
                result.OutgoingError = "Connection test failed";
                result.Detail = "Connection test failed";
            }

            return result;
        }
        catch (Exception ex)
        {
            result.IncomingSuccess = false;
            result.OutgoingSuccess = false;
            result.IncomingError = $"Connection test failed: {ex.Message}";
            result.OutgoingError = $"Connection test failed: {ex.Message}";
            result.Detail = ex.ToString();
            return result;
        }
    }
}
