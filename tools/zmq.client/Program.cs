using System.Buffers.Binary;
using System.Text;
using NBitcoin;
using NBitcoin.DataEncoders;
using NetMQ;
using NetMQ.Sockets;

using var subscriber = new SubscriberSocket();
subscriber.Connect("tcp://127.0.0.1:28332");
subscriber.Subscribe("rawtx");
subscriber.Subscribe("rawblock");
subscriber.Subscribe("sequence");

while (true)
{
    // spec https://github.com/bitcoin/bitcoin/blob/master/doc/zmq.md
    var parts = subscriber.ReceiveMultipartBytes();
    var topic = Encoding.ASCII.GetString(parts[0]);
    var sequence = ToUInt32LittleEndian(parts[2]);
    var body = topic switch
    {
        "rawblock" => ToBlock(parts[1]),
        "rawtx" => ToTx(parts[1]),
        "sequence" => ToSequence(parts[1]),
        _ => throw new Exception("Unknown topic")
    };

    Console.WriteLine("----NEW MESSAGE----");
    Console.WriteLine("Topic: {0}", topic);
    Console.WriteLine("Body: {0}", body);
    Console.WriteLine("Sequence: {0}", sequence);
}

string? ToBlock(byte[] part)
{
    var block = Block.Load(part, Network.TestNet);
    return block.Header.ToString();
}

string? ToTx(byte[] part)
{
    var transaction = Transaction.Load(part, Network.TestNet);
    return transaction.ToString();
}

uint ToUInt32LittleEndian(byte[] part)
{
    return BinaryPrimitives.ReadUInt32LittleEndian(part);
}

string ToSequence(byte[] part)
{
    var hash =  Encoders.Hex.EncodeData(part[..32]);
    var command = (char) part[32];
    var sequence = command is 'R' or 'A' ? ToUInt32LittleEndian(part[33..]) : 0u;
    return $"{command} {hash} {sequence}";
}