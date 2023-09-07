using System.Buffers;
using System.Security.Cryptography;

namespace cryptobank.api.features.accounts.services;

[Singleton<IAccountIdGenerator>]
internal class AccountIdGenerator : IAccountIdGenerator
{
    private const int AccountIdLength = 18;

    public string GenerateAccountId()
    {
        var buffer = ArrayPool<byte>.Shared.Rent(AccountIdLength);

        try
        {
            RandomNumberGenerator.Fill(buffer.AsSpan()[..AccountIdLength]);

            return BitConverter
                .ToString(buffer, 0, AccountIdLength)
                .Replace("-", "");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}