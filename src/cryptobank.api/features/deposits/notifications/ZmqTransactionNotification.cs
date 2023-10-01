using NBitcoin;

namespace cryptobank.api.features.deposits.notifications;

/// <summary>
/// https://github.com/bitcoin/bitcoin/blob/master/doc/zmq.md#rawtx
/// </summary>
internal sealed class ZmqTransactionNotification : INotification
{
    private readonly byte[] _body;

    public ZmqTransactionNotification(byte[] body)
    {
        _body = body;
    }

    public Transaction ToTransaction(Network network) => Transaction.Load(_body, network);
}