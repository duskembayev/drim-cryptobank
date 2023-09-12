namespace cryptobank.api.utils.environment;

public interface IRndBytesGenerator
{
    byte[] GetBytes(int count);
    void Fill(Span<byte> span);
}