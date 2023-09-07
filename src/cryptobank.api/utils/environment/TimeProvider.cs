using Microsoft.AspNetCore.Authentication;

namespace cryptobank.api.utils.environment;

[Singleton<ITimeProvider>]
internal sealed class TimeProvider : ITimeProvider
{
    private readonly ISystemClock _systemClock;

    public TimeProvider(ISystemClock systemClock)
    {
        _systemClock = systemClock;
    }

    public DateOnly Today => DateOnly.FromDateTime(_systemClock.UtcNow.UtcDateTime);
    public DateTime UtcNow => _systemClock.UtcNow.UtcDateTime;
}