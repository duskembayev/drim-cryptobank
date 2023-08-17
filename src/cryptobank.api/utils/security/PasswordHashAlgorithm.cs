using System.Security.Cryptography;
using Enhanced.DependencyInjection;

namespace cryptobank.api.utils.security;

[ContainerEntry(ServiceLifetime.Singleton, typeof(IPasswordHashAlgorithm))]
internal sealed class PasswordHashAlgorithm : IPasswordHashAlgorithm
{
    private const int Iterations = 12768;

    public byte[] GenerateSalt(int length)
    {
        var salt = new byte[length];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(salt);
        return salt;
    }

    public byte[] ComputeHash(string data, byte[] salt, int length)
    {
        return Rfc2898DeriveBytes.Pbkdf2(data, salt, Iterations, HashAlgorithmName.SHA512, length);
    }

    public bool ValidateHash(string data, byte[] hash, byte[] salt)
    {
        var newHash = ComputeHash(data, salt, hash.Length);
        return CryptographicOperations.FixedTimeEquals(hash, newHash);
    }
}