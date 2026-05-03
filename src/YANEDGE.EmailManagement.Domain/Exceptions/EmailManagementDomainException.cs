using System;
using Volo.Abp;

namespace YANEDGE.EmailManagement.Exceptions;

/// <summary>
/// 邮件管理系统领域异常基类
/// </summary>
public class EmailManagementDomainException : BusinessException
{
    public EmailManagementDomainException(
        string code,
        string? message = null,
        string? details = null,
        Exception? innerException = null)
        : base(code, message, details, innerException)
    {
    }
}
