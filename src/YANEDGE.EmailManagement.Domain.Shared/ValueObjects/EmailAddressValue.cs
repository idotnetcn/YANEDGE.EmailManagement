using YANEDGE.EmailManagement.Constants;

namespace YANEDGE.EmailManagement.Domain.Shared.ValueObjects;

/// <summary>
/// 邮箱地址值对象
/// </summary>
public sealed record EmailAddressValue
{
    /// <summary>
    /// 原始邮箱地址
    /// </summary>
    public string Address { get; private init; }

    /// <summary>
    /// 标准化邮箱地址(小写)
    /// </summary>
    public string NormalizedAddress { get; private init; }

    /// <summary>
    /// 显示名
    /// </summary>
    public string? DisplayName { get; private init; }

    private EmailAddressValue()
    {
        // For ORM
        Address = string.Empty;
        NormalizedAddress = string.Empty;
    }

    public EmailAddressValue(string address, string? displayName = null)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Email address cannot be empty", nameof(address));
        }

        if (address.Length > EmailManagementConstants.MaxEmailAddressLength)
        {
            throw new ArgumentException($"Email address cannot exceed {EmailManagementConstants.MaxEmailAddressLength} characters", nameof(address));
        }

        Address = address.Trim();
        NormalizedAddress = Address.ToLowerInvariant();
        DisplayName = displayName?.Trim();

        if (!IsValidEmail(Address))
        {
            throw new ArgumentException("Invalid email address format", nameof(address));
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    // Equality comparison is handled by record type automatically
}
