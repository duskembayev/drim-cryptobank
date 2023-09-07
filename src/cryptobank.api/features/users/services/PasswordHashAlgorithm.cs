using System.Security.Cryptography;
using cryptobank.api.features.users.config;
using Konscious.Security.Cryptography;

namespace cryptobank.api.features.users.services;

[Singleton<IPasswordHashAlgorithm>]
internal sealed class PasswordHashAlgorithm : IPasswordHashAlgorithm
{
    private readonly IOptions<PasswordHashOptions> _options;

    public PasswordHashAlgorithm(IOptions<PasswordHashOptions> options)
    {
        _options = options;
    }

    public async Task<string> HashAsync(string password)
    {
        var salt = GenerateSalt(_options.Value.SaltSize);
        var hash = await ComputeHashAsync(
            Encoding.UTF8.GetBytes(password), salt,
            _options.Value.HashSize,
            _options.Value.DegreeOfParallelism,
            _options.Value.Iterations,
            _options.Value.MemorySize);

        return FormatHash(
            hash, salt,
            _options.Value.DegreeOfParallelism,
            _options.Value.Iterations,
            _options.Value.MemorySize);
    }

    public async Task<bool> ValidateAsync(string delegatePassword, string formattedHash)
    {
        var (degreeOfParallelism, iterations, memorySize, salt, hash) = ParseHash(formattedHash);

        var delegateHash = await ComputeHashAsync(
            Encoding.UTF8.GetBytes(delegatePassword), salt,
            hash.Length,
            degreeOfParallelism,
            iterations,
            memorySize);

        return CryptographicOperations.FixedTimeEquals(hash, delegateHash);
    }

    private static async ValueTask<byte[]> ComputeHashAsync(
        byte[] password, byte[] salt, int hashSize,
        int degreeOfParallelism, int iterations, int memorySize)
    {
        using var argon2 = new Argon2id(password)
        {
            Salt = salt,
            DegreeOfParallelism = degreeOfParallelism,
            Iterations = iterations,
            MemorySize = memorySize
        };

        return await argon2.GetBytesAsync(hashSize);
    }

    private static byte[] GenerateSalt(int length)
    {
        var salt = new byte[length];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(salt);
        return salt;
    }

    private static string FormatHash(byte[] hash, byte[] salt, int degreeOfParallelism, int iterations, int memorySize)
    {
        return new StringBuilder("$argon2id$v=")
            .Append(1)
            .Append("$m=")
            .Append(memorySize)
            .Append(",t=")
            .Append(iterations)
            .Append(",p=")
            .Append(degreeOfParallelism)
            .Append('$')
            .Append(Convert.ToBase64String(salt))
            .Append('$')
            .Append(Convert.ToBase64String(hash))
            .ToString();
    }

    private static (int degreeOfParallelism, int iterations, int memorySize, byte[] salt, byte[] hash) ParseHash(
        string formattedHash)
    {
        var parts = formattedHash.Split('$');
        if (parts.Length != 6)
            throw new FormatException("Invalid hash format");

        if (parts[1] != "argon2id")
            throw new NotSupportedException("Invalid hash format");

        var parameters = parts[3].Split(',');
        if (parameters.Length != 3)
            throw new FormatException("Invalid hash format");

        var memorySize = parameters[0][..2] == "m="
            ? int.Parse(parameters[0][2..])
            : throw new FormatException("Invalid hash format");

        var iterations = parameters[1][..2] == "t="
            ? int.Parse(parameters[1][2..])
            : throw new FormatException("Invalid hash format");

        var degreeOfParallelism = parameters[2][..2] == "p="
            ? int.Parse(parameters[2][2..])
            : throw new FormatException("Invalid hash format");

        var salt = Convert.FromBase64String(parts[4]);
        var hash = Convert.FromBase64String(parts[5]);

        return (degreeOfParallelism, iterations, memorySize, salt, hash);
    }
}