using System.Buffers;
using System.Security.Cryptography;

namespace cryptobank.api.features.accounts.services;

[ContainerEntry(ServiceLifetime.Singleton, typeof(IAccountIdGenerator))]
internal class AccountIdGenerator : IAccountIdGenerator
{
    public string GenerateAccountId()
    {
        var buffer = ArrayPool<byte>.Shared.Rent(16);

        try
        {
            RandomNumberGenerator.Fill(buffer.AsSpan()[..16]);

            return BitConverter
                .ToString(buffer, 0, 16)
                .Replace("-", "");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}