using System;
using System.Security.Cryptography;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace YANEDGE.EmailManagement.Domain.Services.Implementations;

/// <summary>
/// 密码加密服务实现
/// 使用AES-256-GCM进行加密
/// </summary>
public class PasswordEncryptionService : IPasswordEncryptionService, ITransientDependency
{
    private const int KeySize = 32; // 256 bits
    private const int NonceSize = 12; // 96 bits
    private const int TagSize = 16; // 128 bits

    private readonly byte[] _encryptionKey;

    public PasswordEncryptionService()
    {
        // TODO: 实际部署时应从配置中读取加密密钥，或使用密钥管理服务(如Azure Key Vault)
        // 这里使用环境变量或默认密钥（仅用于开发）
        var keyString = Environment.GetEnvironmentVariable("EMAIL_PASSWORD_ENCRYPTION_KEY")
                       ?? "DefaultDevelopmentKey32Chars!";

        _encryptionKey = DeriveKey(keyString);
    }

    public string Encrypt(string plainPassword)
    {
        if (string.IsNullOrEmpty(plainPassword))
        {
            throw new ArgumentException("密码不能为空", nameof(plainPassword));
        }

        var plainBytes = Encoding.UTF8.GetBytes(plainPassword);
        var nonce = new byte[NonceSize];
        var tag = new byte[TagSize];
        var cipherBytes = new byte[plainBytes.Length];

        RandomNumberGenerator.Fill(nonce);

        using var aes = new AesGcm(_encryptionKey, TagSize);
        aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

        // 格式: nonce + tag + ciphertext
        var result = new byte[NonceSize + TagSize + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
        Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
        Buffer.BlockCopy(cipherBytes, 0, result, NonceSize + TagSize, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string encryptedPassword)
    {
        if (string.IsNullOrEmpty(encryptedPassword))
        {
            throw new ArgumentException("加密密码不能为空", nameof(encryptedPassword));
        }

        try
        {
            var encryptedBytes = Convert.FromBase64String(encryptedPassword);

            if (encryptedBytes.Length < NonceSize + TagSize)
            {
                throw new ArgumentException("加密数据格式不正确");
            }

            var nonce = new byte[NonceSize];
            var tag = new byte[TagSize];
            var cipherBytes = new byte[encryptedBytes.Length - NonceSize - TagSize];

            Buffer.BlockCopy(encryptedBytes, 0, nonce, 0, NonceSize);
            Buffer.BlockCopy(encryptedBytes, NonceSize, tag, 0, TagSize);
            Buffer.BlockCopy(encryptedBytes, NonceSize + TagSize, cipherBytes, 0, cipherBytes.Length);

            var plainBytes = new byte[cipherBytes.Length];

            using var aes = new AesGcm(_encryptionKey, TagSize);
            aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("解密失败", ex);
        }
    }

    public bool Verify(string plainPassword, string encryptedPassword)
    {
        if (string.IsNullOrEmpty(plainPassword) || string.IsNullOrEmpty(encryptedPassword))
        {
            return false;
        }

        try
        {
            var decrypted = Decrypt(encryptedPassword);
            return decrypted == plainPassword;
        }
        catch
        {
            return false;
        }
    }

    private static byte[] DeriveKey(string keyString)
    {
        // 使用PBKDF2从字符串派生固定长度的密钥
        using var pbkdf2 = new Rfc2898DeriveBytes(
            keyString,
            Encoding.UTF8.GetBytes("EmailManagementSalt"), // 固定盐值
            10000,
            HashAlgorithmName.SHA256);

        return pbkdf2.GetBytes(KeySize);
    }
}
