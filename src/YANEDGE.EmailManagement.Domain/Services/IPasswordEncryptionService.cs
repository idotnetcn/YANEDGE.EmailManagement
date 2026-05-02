namespace YANEDGE.EmailManagement.Domain.Services;

/// <summary>
/// 密码加密服务接口
/// </summary>
public interface IPasswordEncryptionService
{
    /// <summary>
    /// 加密密码
    /// </summary>
    string Encrypt(string plainPassword);

    /// <summary>
    /// 解密密码
    /// </summary>
    string Decrypt(string encryptedPassword);

    /// <summary>
    /// 验证密码(用于测试连接)
    /// </summary>
    bool Verify(string plainPassword, string encryptedPassword);
}
