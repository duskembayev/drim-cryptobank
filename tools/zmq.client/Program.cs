using System.Text;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.RPC;
using NBitcoin.Scripting;
using NetMQ;
using NetMQ.Sockets;

using var subscriber = new SubscriberSocket();
subscriber.Connect("tcp://127.0.0.1:28332");
subscriber.Subscribe("rawtx");
subscriber.Subscribe("rawblock");

while (true)
{
    var parts = subscriber.ReceiveMultipartBytes();
    var topic = Encoding.UTF8.GetString(parts[0]);
    var suffix = ToHex(parts[2]);
    var body = topic switch
    {
        "rawblock" => ToBlock(parts[1]),
        "rawtx" => ToTx(parts[1]),
        _ => throw new Exception("Unknown topic")
    };

    Console.WriteLine("----NEW MESSAGE----");
    Console.WriteLine("Topic: {0}", topic);
    Console.WriteLine("Body: {0}", body);
    Console.WriteLine("Suffix: {0}", suffix);
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

string ToHex(byte[] part)
{
    return Encoders.Hex.EncodeData(part);
}