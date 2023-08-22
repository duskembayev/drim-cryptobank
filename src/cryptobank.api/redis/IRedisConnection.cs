using StackExchange.Redis;

namespace cryptobank.api.redis;

public interface IRedisConnection
{
    IDatabase Database { get; }
}