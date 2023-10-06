using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace cryptobank.api.tests.harnesses.core;

internal abstract class Harness : IHarness<Program>
{
    private WebApplicationFactory<Program>? _factory;
    private bool _started;

    protected WebApplicationFactory<Program> Factory
    {
        get
        {
            ThrowIfNotStarted();
            return _factory;
        }
    }

    void IHarness<Program>.Configure(IWebHostBuilder builder)
    {
        OnConfigure(builder);
    }

    Task IHarness<Program>.StartAsync(WebApplicationFactory<Program> factory, CancellationToken cancellationToken)
    {
        _factory = factory;
        _started = true;
        return OnStartAsync(cancellationToken);
    }

    Task IHarness<Program>.StopAsync(CancellationToken cancellationToken)
    {
        _started = false;
        return OnStopAsync(cancellationToken);
    }

    protected virtual void OnConfigure(IWebHostBuilder builder)
    {
    }

    protected virtual Task OnStartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task OnStopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    [MemberNotNull(nameof(_factory))]
    public void ThrowIfNotStarted()
    {
        if (!_started)
            throw new InvalidOperationException($"Harness is not started. Call {nameof(IHarness<Program>.StartAsync)} first.");

        Assert.NotNull(_factory);
    }
}