namespace cryptobank.api.db;

public class DbOptions
{
    public bool RestoreEnabled { get; init; }
    public TimeSpan RestoreTimeout { get; init; }
}