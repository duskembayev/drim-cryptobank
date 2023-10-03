using System.Buffers;

namespace cryptobank.api.utils.environment;

public interface IRndBytesGenerator
{
    byte[] GetBytes(int count);

    void Fill(Span<byte> span);

    string GetAsBase64(int count)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(count);

        try
        {
            Fill(buffer.AsSpan()[..count]);
            return Convert.ToBase64String(buffer, 0, count);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}