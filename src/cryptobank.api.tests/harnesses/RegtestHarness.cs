using cryptobank.api.features.deposits.services;
using cryptobank.api.tests.harnesses.core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NBitcoin;
using NBitcoin.RPC;

namespace cryptobank.api.tests.harnesses;

internal class RegtestHarness : Harness
{
    private const int ZmqPort = 28332;
    private const string RpcUser = "rpc_user";
    private const string RpcPassword = "P@$sw0rd";
    private static readonly int RpcPort = Network.RegTest.RPCPort;
    
    private IContainer? _container;
    private ushort? _publicRpcPort;
    private ushort? _publicZmqPort;

    protected override void OnConfigure(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                {
                    $"ConnectionStrings:{ZmqSubscription.ConnectionStringName}",
                    $"tcp://localhost:{_publicZmqPort}"
                },
                {
                    $"ConnectionStrings:{RpcClient.ConnectionStringName}",
                    $"server=http://localhost:{_publicRpcPort};{RpcUser}:{RpcPassword}"
                },
                {
                    "Features:Deposits:Network", Network.RegTest.Name
                }
            });
        });
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        var confFile = new FileInfo("bitcoin/bitcoin.conf");
        Assert.True(confFile.Exists);

        _container = new ContainerBuilder()
            .WithImage("kylemanna/bitcoind")
            .WithResourceMapping(confFile, "/bitcoin/.bitcoin/")
            .WithPortBinding(RpcPort, true)
            .WithPortBinding(ZmqPort, true)
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .UntilPortIsAvailable(RpcPort)
                .UntilPortIsAvailable(ZmqPort))
            .WithCommand("-chain=regtest", $"-rpcuser={RpcUser}", $"-rpcpassword={RpcPassword}")
            .Build();

        await _container.StartAsync(cancellationToken);
        
        _publicRpcPort = _container.GetMappedPublicPort(RpcPort);
        _publicZmqPort = _container.GetMappedPublicPort(ZmqPort);
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        if (_container is not null)
        {
            await _container.StopAsync(cancellationToken);
            await _container.DisposeAsync();
        }
    }

    public RPCClient CreateClient()
    {
        var rpcCredentialString = new RPCCredentialString
        {
            Server = $"http://localhost:{_publicRpcPort}",
            UserPassword = new NetworkCredential(RpcUser, RpcPassword)
        };

        return new RPCClient(rpcCredentialString, Network.RegTest);
    }
}