using System.Security.Cryptography;

namespace cryptobank.api.utils.environment;

[Singleton<IRndBytesGenerator>]
internal sealed class RndBytesGenerator : IRndBytesGenerator
{
    public byte[] GetBytes(int count) => RandomNumberGenerator.GetBytes(count);
    public void Fill(Span<byte> span) => RandomNumberGenerator.Fill(span);
}