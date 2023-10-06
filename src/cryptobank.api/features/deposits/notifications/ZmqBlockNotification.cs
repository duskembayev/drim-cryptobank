using NBitcoin;

namespace cryptobank.api.features.deposits.notifications;

/// <summary>
/// https://github.com/bitcoin/bitcoin/blob/master/doc/zmq.md#rawblock
/// </summary>
internal sealed class ZmqBlockNotification : INotification
{
    private readonly byte[] _body;

    public ZmqBlockNotification(byte[] body)
    {
        _body = body;
    }

    public Block ToBlock(Network network) => Block.Load(_body, network);
}