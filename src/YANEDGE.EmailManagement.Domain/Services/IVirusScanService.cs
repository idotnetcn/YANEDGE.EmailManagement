using System;
using System.Threading.Tasks;

namespace YANEDGE.EmailManagement.Services;

/// <summary>
/// 病毒扫描服务接口
/// </summary>
public interface IVirusScanService
{
    /// <summary>
    /// 扫描文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>扫描结果</returns>
    Task<VirusScanResult> ScanFileAsync(string filePath);

    /// <summary>
    /// 扫描字节数组
    /// </summary>
    /// <param name="fileBytes">文件字节</param>
    /// <param name="fileName">文件名（用于日志）</param>
    /// <returns>扫描结果</returns>
    Task<VirusScanResult> ScanBytesAsync(byte[] fileBytes, string fileName);

    /// <summary>
    /// 检查扫描服务是否可用
    /// </summary>
    /// <returns>是否可用</returns>
    Task<bool> IsAvailableAsync();
}

/// <summary>
/// 病毒扫描结果
/// </summary>
public class VirusScanResult
{
    /// <summary>
    /// 是否安全（未检测到病毒）
    /// </summary>
    public bool IsSafe { get; set; }

    /// <summary>
    /// 检测到的病毒名称（如果有）
    /// </summary>
    public string? DetectedVirus { get; set; }

    /// <summary>
    /// 扫描详细信息
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// 扫描时间
    /// </summary>
    public DateTime ScanTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 扫描是否成功执行
    /// </summary>
    public bool ScanSucceeded { get; set; } = true;

    /// <summary>
    /// 错误信息（如果扫描失败）
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 创建安全的扫描结果
    /// </summary>
    public static VirusScanResult Safe(string details = "未检测到威胁")
    {
        return new VirusScanResult
        {
            IsSafe = true,
            Details = details,
            ScanSucceeded = true
        };
    }

    /// <summary>
    /// 创建不安全的扫描结果
    /// </summary>
    public static VirusScanResult Unsafe(string detectedVirus, string details)
    {
        return new VirusScanResult
        {
            IsSafe = false,
            DetectedVirus = detectedVirus,
            Details = details,
            ScanSucceeded = true
        };
    }

    /// <summary>
    /// 创建失败的扫描结果
    /// </summary>
    public static VirusScanResult Failed(string errorMessage)
    {
        return new VirusScanResult
        {
            IsSafe = false,
            ScanSucceeded = false,
            ErrorMessage = errorMessage,
            Details = "扫描失败"
        };
    }
}
