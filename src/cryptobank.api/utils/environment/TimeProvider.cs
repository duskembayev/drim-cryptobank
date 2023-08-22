namespace cryptobank.api.utils.environment;

[ContainerEntry(ServiceLifetime.Singleton, typeof(ITimeProvider))]
internal sealed class TimeProvider : ITimeProvider
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
    public DateTime UtcNow => DateTime.UtcNow;
}