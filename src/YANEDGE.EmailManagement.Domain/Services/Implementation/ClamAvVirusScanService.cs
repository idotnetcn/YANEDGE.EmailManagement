using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace YANEDGE.EmailManagement.Services.Implementation;

/// <summary>
/// ClamAV病毒扫描服务实现
/// 使用ClamAV命令行工具进行病毒扫描
/// </summary>
public class ClamAvVirusScanService : IVirusScanService, ITransientDependency
{
    private readonly ILogger<ClamAvVirusScanService> _logger;
    private readonly ClamAvOptions _options;

    public ClamAvVirusScanService(
        ILogger<ClamAvVirusScanService> logger,
        ClamAvOptions options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task<VirusScanResult> ScanFileAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                _logger.LogError("要扫描的文件不存在: {FilePath}", filePath);
                return VirusScanResult.Failed($"文件不存在: {filePath}");
            }

            if (!_options.Enabled)
            {
                _logger.LogWarning("病毒扫描已禁用，跳过扫描");
                return VirusScanResult.Safe("病毒扫描已禁用");
            }

            // 检查ClamAV是否可用
            if (!await IsAvailableAsync())
            {
                _logger.LogError("ClamAV不可用");
                return VirusScanResult.Failed("病毒扫描服务不可用");
            }

            _logger.LogInformation("开始扫描文件: {FilePath}", filePath);

            // 使用clamdscan（更快）或clamscan
            var clamCommand = _options.UseClamdScan ? "clamdscan" : "clamscan";
            var arguments = $"--no-summary \"{filePath}\"";

            var processStartInfo = new ProcessStartInfo
            {
                FileName = clamCommand,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    outputBuilder.AppendLine(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    errorBuilder.AppendLine(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // 等待扫描完成，最多等待配置的超时时间
            using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(_options.ScanTimeoutSeconds));
            try
            {
                await process.WaitForExitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("ClamAV扫描超时: {FilePath}", filePath);
                process.Kill(true);
                return VirusScanResult.Failed("扫描超时");
            }

            var output = outputBuilder.ToString();
            var error = errorBuilder.ToString();

            _logger.LogDebug("ClamAV扫描输出: {Output}", output);

            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogWarning("ClamAV扫描错误: {Error}", error);
            }

            // 分析扫描结果
            // ClamAV退出码: 0=未发现病毒, 1=发现病毒, 2=错误
            if (process.ExitCode == 0)
            {
                _logger.LogInformation("文件扫描完成，未检测到威胁: {FilePath}", filePath);
                return VirusScanResult.Safe($"文件已扫描，未检测到威胁");
            }
            else if (process.ExitCode == 1)
            {
                // 解析病毒名称
                var virusName = ParseVirusName(output);
                _logger.LogWarning("文件扫描完成，检测到病毒: {VirusName} in {FilePath}", virusName, filePath);
                return VirusScanResult.Unsafe(virusName, $"检测到病毒: {virusName}");
            }
            else
            {
                _logger.LogError("ClamAV扫描失败，退出码: {ExitCode}", process.ExitCode);
                return VirusScanResult.Failed($"扫描失败，退出码: {process.ExitCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "病毒扫描过程中发生异常: {FilePath}", filePath);
            return VirusScanResult.Failed($"扫描异常: {ex.Message}");
        }
    }

    public async Task<VirusScanResult> ScanBytesAsync(byte[] fileBytes, string fileName)
    {
        try
        {
            // 将字节写入临时文件进行扫描
            var tempFilePath = Path.Combine(Path.GetTempPath(), $"scan_{Guid.NewGuid()}_{fileName}");

            try
            {
                await File.WriteAllBytesAsync(tempFilePath, fileBytes);
                return await ScanFileAsync(tempFilePath);
            }
            finally
            {
                // 清理临时文件
                if (File.Exists(tempFilePath))
                {
                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "无法删除临时扫描文件: {FilePath}", tempFilePath);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "扫描字节数组时发生异常");
            return VirusScanResult.Failed($"扫描异常: {ex.Message}");
        }
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            if (!_options.Enabled)
            {
                return false;
            }

            // 检查ClamAV是否已安装
            var clamCommand = _options.UseClamdScan ? "clamdscan" : "clamscan";
            var processStartInfo = new ProcessStartInfo
            {
                FileName = clamCommand,
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };
            process.Start();

            using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(5));
            try
            {
                await process.WaitForExitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                return false;
            }

            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "检查ClamAV可用性时发生异常");
            return false;
        }
    }

    /// <summary>
    /// 从ClamAV输出中解析病毒名称
    /// </summary>
    private string ParseVirusName(string output)
    {
        try
        {
            // ClamAV输出格式: /path/to/file: Virus.Name FOUND
            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.Contains("FOUND", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = line.Split(':');
                    if (parts.Length >= 2)
                    {
                        var virusPart = parts[1].Trim();
                        virusPart = virusPart.Replace("FOUND", "", StringComparison.OrdinalIgnoreCase).Trim();
                        return virusPart;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "解析病毒名称时发生异常");
        }

        return "未知病毒";
    }
}

/// <summary>
/// ClamAV配置选项
/// </summary>
public class ClamAvOptions
{
    /// <summary>
    /// 是否启用病毒扫描
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 使用clamdscan（守护进程模式）而不是clamscan
    /// clamdscan更快，但需要clamd守护进程运行
    /// </summary>
    public bool UseClamdScan { get; set; } = true;

    /// <summary>
    /// 扫描超时时间（秒）
    /// </summary>
    public int ScanTimeoutSeconds { get; set; } = 300;

    /// <summary>
    /// 最大文件大小（MB）
    /// 超过此大小的文件将跳过扫描
    /// </summary>
    public int MaxFileSizeMB { get; set; } = 100;
}
