using System.Buffers;

namespace cryptobank.api.features.accounts.services;

[Singleton<IAccountIdGenerator>]
internal class AccountIdGenerator : IAccountIdGenerator
{
    private const int AccountIdLength = 18;
    private readonly IRndBytesGenerator _rndBytesGenerator;

    public AccountIdGenerator(IRndBytesGenerator rndBytesGenerator)
    {
        _rndBytesGenerator = rndBytesGenerator;
    }

    public string GenerateAccountId()
    {
        var buffer = ArrayPool<byte>.Shared.Rent(AccountIdLength);

        try
        {
            _rndBytesGenerator.Fill(buffer.AsSpan(..AccountIdLength));
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