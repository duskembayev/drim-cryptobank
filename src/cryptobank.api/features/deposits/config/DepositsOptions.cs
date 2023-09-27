using NBitcoin;

namespace cryptobank.api.features.deposits.config;

[Options("Deposits")]
public class DepositsOptions
{
    public required string Xpub { get; init; }
    public required Network Network { get; init; } = Network.TestNet;
}