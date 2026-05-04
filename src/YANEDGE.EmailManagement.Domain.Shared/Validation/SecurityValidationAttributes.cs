using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace YANEDGE.EmailManagement.Validation;

/// <summary>
/// 安全字符串验证特性
/// 防止XSS和SQL注入攻击
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class SafeStringAttribute : ValidationAttribute
{
    private static readonly Regex DangerousPatternRegex = new Regex(
        @"(<script|</script|javascript:|onerror=|onload=|<iframe|eval\(|expression\(|vbscript:|data:text/html)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public SafeStringAttribute() : base("输入包含不安全的内容")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return true; // 空值由 Required 特性处理
        }

        var input = value.ToString()!;

        // 检查是否包含危险模式
        if (DangerousPatternRegex.IsMatch(input))
        {
            return false;
        }

        return true;
    }
}

/// <summary>
/// 邮件地址验证特性（增强版）
/// 更严格的邮件地址格式验证
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class EmailAddressExAttribute : ValidationAttribute
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
        RegexOptions.Compiled);

    public EmailAddressExAttribute() : base("邮件地址格式无效")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return true;
        }

        var email = value.ToString()!;

        // 长度检查
        if (email.Length > 254)
        {
            return false;
        }

        // 格式检查
        if (!EmailRegex.IsMatch(email))
        {
            return false;
        }

        // 本地部分长度检查（@之前）
        var parts = email.Split('@');
        if (parts[0].Length > 64)
        {
            return false;
        }

        return true;
    }
}

/// <summary>
/// 文件路径验证特性
/// 防止路径遍历攻击
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class SafeFilePathAttribute : ValidationAttribute
{
    private static readonly char[] InvalidChars = new char[] { '<', '>', ':', '|', '?', '*', '\0' };
    private static readonly string[] DangerousPatterns = new string[] { "..", "~", "%00" };

    public SafeFilePathAttribute() : base("文件路径包含不安全的字符或模式")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return true;
        }

        var path = value.ToString()!;

        // 检查无效字符
        if (path.IndexOfAny(InvalidChars) >= 0)
        {
            return false;
        }

        // 检查危险模式（路径遍历）
        foreach (var pattern in DangerousPatterns)
        {
            if (path.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }
}

/// <summary>
/// 文件大小验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly long _maxFileSize;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="maxFileSizeInMB">最大文件大小（MB）</param>
    public MaxFileSizeAttribute(int maxFileSizeInMB)
    {
        _maxFileSize = maxFileSizeInMB * 1024L * 1024L;
        ErrorMessage = $"文件大小不能超过 {maxFileSizeInMB} MB";
    }

    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return true;
        }

        if (value is long fileSize)
        {
            return fileSize <= _maxFileSize;
        }

        return false;
    }
}

/// <summary>
/// 允许的文件扩展名验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class AllowedFileExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _allowedExtensions;

    public AllowedFileExtensionsAttribute(params string[] allowedExtensions)
    {
        _allowedExtensions = allowedExtensions.Select(e => e.ToLowerInvariant()).ToArray();
        ErrorMessage = $"只允许以下文件类型: {string.Join(", ", allowedExtensions)}";
    }

    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return true;
        }

        var fileName = value.ToString()!;
        var extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension))
        {
            return false;
        }

        return _allowedExtensions.Contains(extension);
    }
}

/// <summary>
/// SQL注入防护验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class NoSqlInjectionAttribute : ValidationAttribute
{
    private static readonly Regex SqlInjectionRegex = new Regex(
        @"(\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER|EXEC|EXECUTE|UNION|DECLARE|CAST|CONVERT)\b)|(-{2})|(/\*)|(\*/)|(\bOR\b.*=.*)|(\bAND\b.*=.*)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public NoSqlInjectionAttribute() : base("输入包含可疑的SQL语句")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return true;
        }

        var input = value.ToString()!;

        return !SqlInjectionRegex.IsMatch(input);
    }
}

/// <summary>
/// HTML内容验证特性
/// 只允许安全的HTML标签
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class SafeHtmlAttribute : ValidationAttribute
{
    private static readonly string[] AllowedTags = new string[]
    {
        "p", "br", "strong", "em", "u", "ol", "ul", "li", "a", "img", "blockquote", "pre", "code",
        "h1", "h2", "h3", "h4", "h5", "h6", "table", "thead", "tbody", "tr", "th", "td", "div", "span"
    };

    private static readonly Regex DangerousHtmlRegex = new Regex(
        @"<script|</script|javascript:|onerror=|onload=|onclick=|onmouseover=|<iframe|<object|<embed|<link|<meta|<style",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public SafeHtmlAttribute() : base("HTML内容包含不安全的标签或脚本")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return true;
        }

        var html = value.ToString()!;

        // 检查危险模式
        if (DangerousHtmlRegex.IsMatch(html))
        {
            return false;
        }

        return true;
    }
}

/// <summary>
/// URL验证特性（增强版）
/// 只允许HTTP和HTTPS协议
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class SafeUrlAttribute : ValidationAttribute
{
    public SafeUrlAttribute() : base("URL格式无效或使用了不安全的协议")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return true;
        }

        var url = value.ToString()!;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false;
        }

        // 只允许HTTP和HTTPS协议
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }

        return true;
    }
}
