using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace cryptobank.api.tests.harnesses.core;

public interface IHarness<TEntryPoint> where TEntryPoint : class
{
    void Configure(IWebHostBuilder builder);

    Task StartAsync(WebApplicationFactory<TEntryPoint> factory, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}