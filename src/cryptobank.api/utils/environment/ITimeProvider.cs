namespace cryptobank.api.utils.environment;

public interface ITimeProvider
{
    DateOnly Today { get; }
    DateTime UtcNow { get; }
}