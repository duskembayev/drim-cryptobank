namespace cryptobank.api.features.users.config;

public class PasswordHashOptions
{
    public int DegreeOfParallelism { get; set; } = 16;
    public int Iterations { get; set; } = 40;
    public int MemorySize { get; set; } = 8192;
    public int SaltSize { get; set; } = 32;
    public int HashSize { get; set; } = 128;
}   