using System;
using System.Security.Cryptography;
using System.Text;
using Volo.Abp.DependencyInjection;
using YANEDGE.EmailManagement.Domain.Services;

namespace YANEDGE.EmailManagement.Services.Implementation;

/// <summary>
/// 密码加密服务实现
/// 使用AES-256加密算法
/// </summary>
public class PasswordEncryptionService : IPasswordEncryptionService, ITransientDependency
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public PasswordEncryptionService()
    {
        // 在生产环境中，这些密钥应该从配置中读取
        // 这里使用固定密钥仅供演示
        _key = Encoding.UTF8.GetBytes("YANEDGEEmailManagementKey1234"); // 32 bytes for AES-256
        _iv = Encoding.UTF8.GetBytes("YANEDGEInitVect1"); // 16 bytes
    }

    public string Encrypt(string plainPassword)
    {
        if (string.IsNullOrEmpty(plainPassword))
        {
            return plainPassword;
        }

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainPassword);
        }

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public string Decrypt(string encryptedPassword)
    {
        if (string.IsNullOrEmpty(encryptedPassword))
        {
            return encryptedPassword;
        }

        try
        {
            var buffer = Convert.FromBase64String(encryptedPassword);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var msDecrypt = new MemoryStream(buffer);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to decrypt password. The encrypted data may be corrupted.", ex);
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
}
