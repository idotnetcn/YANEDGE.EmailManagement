# 阶段三: 安全加固功能实施文档

## 概述

本文档描述了在YANEDGE邮件管理系统中实施的第三阶段安全加固功能。这些功能旨在增强系统的安全性，防止常见的Web攻击和滥用。

## 实施的功能

### 1. 速率限制 (Rate Limiting)

**目的**: 防止API滥用和DDoS攻击

**实现位置**:
- `src/YANEDGE.EmailManagement.HttpApi.Host/Middleware/RateLimitingMiddleware.cs`

**功能特性**:
- 基于IP地址和已认证用户ID的请求限制
- 支持不同端点的自定义速率限制规则
- 使用Redis分布式缓存实现
- 返回标准HTTP 429状态码和重试信息
- 添加速率限制响应头 (X-RateLimit-*)

**默认规则**:
```
- 默认：100个请求/分钟
- 登录端点：10个请求/5分钟
- 邮件发送：10个请求/分钟
- 附件上传：20个请求/分钟
- 搜索端点：30个请求/分钟
```

**配置项** (`appsettings.json`):
```json
"Security": {
  "RateLimit": {
    "Enabled": true,
    "DefaultMaxRequests": 100,
    "DefaultWindowSeconds": 60
  }
}
```

### 2. 安全头中间件 (Security Headers)

**目的**: 通过HTTP安全响应头防止常见Web攻击

**实现位置**:
- `src/YANEDGE.EmailManagement.HttpApi.Host/Middleware/SecurityHeadersMiddleware.cs`

**添加的安全头**:
- `X-Content-Type-Options: nosniff` - 防止MIME类型嗅探
- `X-Frame-Options: SAMEORIGIN` - 防止点击劫持
- `X-XSS-Protection: 1; mode=block` - 启用XSS过滤器
- `Strict-Transport-Security` - 强制HTTPS (HSTS)
- `Content-Security-Policy` - 内容安全策略
- `Referrer-Policy` - 控制Referer信息
- `Permissions-Policy` - 控制浏览器功能和API
- 移除泄露服务器信息的响应头

**配置项** (`appsettings.json`):
```json
"Security": {
  "SecurityHeaders": {
    "Enabled": true,
    "EnableHsts": true,
    "HstsMaxAge": 31536000,
    "EnableContentSecurityPolicy": true
  }
}
```

### 3. 输入验证增强 (Enhanced Input Validation)

**目的**: 防止XSS、SQL注入、路径遍历等注入攻击

**实现位置**:
- `src/YANEDGE.EmailManagement.Domain.Shared/Validation/SecurityValidationAttributes.cs`

**提供的验证特性**:

1. **SafeStringAttribute** - 防止XSS攻击
   - 检测危险的脚本模式
   - 阻止 `<script>`, `javascript:`, `onerror=` 等

2. **EmailAddressExAttribute** - 增强的邮件地址验证
   - 更严格的格式检查
   - 长度限制（总长度254字符，本地部分64字符）

3. **SafeFilePathAttribute** - 防止路径遍历攻击
   - 阻止 `..`, `~`, `%00` 等危险模式
   - 检查非法文件路径字符

4. **MaxFileSizeAttribute** - 文件大小限制
   - 以MB为单位指定最大文件大小

5. **AllowedFileExtensionsAttribute** - 允许的文件扩展名
   - 白名单方式验证文件类型

6. **NoSqlInjectionAttribute** - SQL注入防护
   - 检测可疑的SQL语句模式

7. **SafeHtmlAttribute** - HTML内容验证
   - 只允许安全的HTML标签
   - 阻止危险的脚本和事件处理器

8. **SafeUrlAttribute** - URL验证
   - 只允许HTTP和HTTPS协议

**使用示例**:
```csharp
public class CreateMailInput
{
    [Required]
    [SafeString]
    [StringLength(500)]
    public string Subject { get; set; }

    [EmailAddressEx]
    public string ToAddress { get; set; }

    [SafeHtml]
    public string Body { get; set; }
}
```

### 4. 附件病毒扫描 (Virus Scanning)

**目的**: 检测和阻止恶意文件上传

**实现位置**:
- `src/YANEDGE.EmailManagement.Domain/Services/IVirusScanService.cs`
- `src/YANEDGE.EmailManagement.Domain/Services/Implementation/ClamAvVirusScanService.cs`

**功能特性**:
- 集成ClamAV开源病毒扫描引擎
- 支持文件路径和字节数组扫描
- 异步扫描处理
- 扫描超时控制
- 详细的扫描结果记录

**接口定义**:
```csharp
public interface IVirusScanService
{
    Task<VirusScanResult> ScanFileAsync(string filePath);
    Task<VirusScanResult> ScanBytesAsync(byte[] fileBytes, string fileName);
    Task<bool> IsAvailableAsync();
}
```

**配置项** (`appsettings.json`):
```json
"Security": {
  "VirusScan": {
    "Enabled": true,
    "UseClamdScan": true,
    "ScanTimeoutSeconds": 300,
    "MaxFileSizeMB": 100
  }
}
```

**ClamAV安装**:
```bash
# Ubuntu/Debian
sudo apt-get update
sudo apt-get install clamav clamav-daemon

# 启动守护进程
sudo systemctl start clamav-daemon
sudo systemctl enable clamav-daemon

# 更新病毒库
sudo freshclam
```

## 集成到应用程序管道

所有安全中间件已集成到 `EmailManagementHttpApiHostModule` 中：

**中间件顺序**:
1. 安全头中间件 (最前面)
2. 全局异常处理
3. 速率限制中间件
4. ABP请求本地化
5. 其他中间件...

```csharp
public override void OnApplicationInitialization(ApplicationInitializationContext context)
{
    var app = context.GetApplicationBuilder();

    // 1. 安全头
    app.UseSecurityHeaders(options);

    // 2. 异常处理
    app.UseGlobalExceptionHandler();

    // 3. 速率限制
    app.UseRateLimiting(options);

    // 4. 其他中间件...
    app.UseAbpRequestLocalization();
    // ...
}
```

## 使用指南

### 应用输入验证特性

在DTO类上使用验证特性：

```csharp
using YANEDGE.EmailManagement.Validation;

public class UploadAttachmentInput
{
    [Required]
    [SafeFilePath]
    public string FileName { get; set; }

    [MaxFileSize(50)] // 50 MB
    [AllowedFileExtensions(".pdf", ".doc", ".docx", ".xls", ".xlsx")]
    public string FileExtension { get; set; }
}
```

### 使用病毒扫描服务

在应用服务中注入和使用：

```csharp
public class MailAttachmentAppService : ApplicationService
{
    private readonly IVirusScanService _virusScanService;

    public MailAttachmentAppService(IVirusScanService virusScanService)
    {
        _virusScanService = virusScanService;
    }

    public async Task<UploadResult> UploadAttachmentAsync(byte[] fileBytes, string fileName)
    {
        // 扫描文件
        var scanResult = await _virusScanService.ScanBytesAsync(fileBytes, fileName);

        if (!scanResult.IsSafe)
        {
            throw new BusinessException("检测到恶意文件！");
        }

        // 继续处理安全文件...
    }
}
```

### 自定义速率限制规则

修改 `RateLimitingMiddleware.cs` 中的 `RateLimitOptions`:

```csharp
public Dictionary<string, RateLimitRule> EndpointRules { get; set; } = new()
{
    ["/api/custom-endpoint"] = new RateLimitRule
    {
        MaxRequests = 50,
        WindowSeconds = 60
    }
};
```

## 测试建议

### 1. 测试速率限制
```bash
# 快速发送多个请求
for i in {1..150}; do
  curl http://localhost:5000/api/mail-messages
done

# 应该在第101个请求后收到429响应
```

### 2. 测试安全头
```bash
# 检查响应头
curl -I http://localhost:5000/api/health

# 应该包含安全响应头
```

### 3. 测试输入验证
```bash
# 尝试注入恶意脚本
curl -X POST http://localhost:5000/api/mail-messages \
  -H "Content-Type: application/json" \
  -d '{"subject":"<script>alert(1)</script>"}'

# 应该返回验证错误
```

### 4. 测试病毒扫描
```bash
# 测试EICAR测试文件
# 创建标准EICAR测试病毒文件并尝试上传
# 应该被检测并拒绝
```

## 性能影响

- **速率限制**: 最小延迟 (<1ms)，使用Redis缓存
- **安全头**: 可忽略不计 (<0.1ms)
- **输入验证**: 最小延迟 (<1ms)，基于正则表达式
- **病毒扫描**: 取决于文件大小，通常 1-5秒

## 注意事项

1. **Redis依赖**: 速率限制需要Redis正常运行
2. **ClamAV依赖**: 病毒扫描需要安装和配置ClamAV
3. **配置灵活性**: 所有安全功能都可以通过配置启用/禁用
4. **生产环境**: 建议在生产环境启用所有安全功能

## 后续改进建议

1. 实现更细粒度的速率限制（基于用户角色）
2. 添加速率限制的管理界面
3. 实现病毒扫描结果的详细日志和报告
4. 添加自动化安全测试套件
5. 考虑集成WAF (Web Application Firewall)

## 参考资源

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [ClamAV Documentation](https://docs.clamav.net/)
- [ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [Content Security Policy](https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP)
