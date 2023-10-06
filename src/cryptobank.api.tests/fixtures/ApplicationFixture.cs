using cryptobank.api.tests.harnesses;
using cryptobank.api.tests.harnesses.core;
using Microsoft.AspNetCore.Mvc.Testing;

namespace cryptobank.api.tests.fixtures;

public class ApplicationFixture : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApplicationFixture()
    {
        Redis = new RedisHarness();
        Postgres = new PostgresHarness();
        Database = new DatabaseHarness(Postgres);
        Regtest = new RegtestHarness();
        HttpClient = new HttpClientHarness();
        Setup = new SetupHarness(Database);

        _factory = new WebApplicationFactory<Program>()
            .AddHarness(Redis)
            .AddHarness(Postgres)
            .AddHarness(Database)
            .AddHarness(Regtest)
            .AddHarness(HttpClient)
            .AddHarness(Setup);
    }

    internal RegtestHarness Regtest { get; }

    internal DatabaseHarness Database { get; }

    internal PostgresHarness Postgres { get; }

    internal RedisHarness Redis { get; }

    internal HttpClientHarness HttpClient { get; }

    internal SetupHarness Setup { get; }

    internal IServiceProvider Services => _factory.Services;

    async Task IAsyncLifetime.InitializeAsync()
    {
        await ((IHarness<Program>) Redis).StartAsync(_factory, default);
        await ((IHarness<Program>) Postgres).StartAsync(_factory, default);
        await ((IHarness<Program>) Database).StartAsync(_factory, default);
        await ((IHarness<Program>) Regtest).StartAsync(_factory, default);
        await ((IHarness<Program>) HttpClient).StartAsync(_factory, default);
        await ((IHarness<Program>) Setup).StartAsync(_factory, default);

        _ = _factory.Server;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await ((IHarness<Program>) Setup).StopAsync(default);
        await ((IHarness<Program>) HttpClient).StopAsync(default);
        await ((IHarness<Program>) Regtest).StopAsync(default);
        await ((IHarness<Program>) Database).StopAsync(default);
        await ((IHarness<Program>) Postgres).StopAsync(default);
        await ((IHarness<Program>) Redis).StopAsync(default);
    }
}