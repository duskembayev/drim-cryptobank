using NBitcoin;

namespace cryptobank.api.features.deposits.notifications;

/// <summary>
/// https://github.com/bitcoin/bitcoin/blob/master/doc/zmq.md#sequence
/// </summary>
internal sealed class ZmqSequenceNotification : INotification
{
    private readonly byte[] _body;

    public ZmqSequenceNotification(byte[] body)
    {
        _body = body;
    }

    public uint256 GetHash() => new(_body[..32]);
    public MessageType GetMessageType() => (MessageType) _body[32];
    
    public enum MessageType : byte
    {
        BlockhashConnected = (byte) 'C',
        BlockhashDisconnected = (byte) 'D',
        TransactionHashRemoved = (byte) 'R',
        TransactionHashAdded = (byte) 'A'
    }
}