namespace cryptobank.api.redis;

public static class SetupExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection @this)
    {
        @this.AddSingleton<RedisConnection>();
        @this.AddSingleton<IRedisConnection>(provider => provider.GetRequiredService<RedisConnection>());
        @this.AddHostedService<RedisConnection>(provider => provider.GetRequiredService<RedisConnection>());

        @this
            .AddHealthChecks()
            .AddCheck<RedisConnection>("Redis");

        return @this;
    }
}