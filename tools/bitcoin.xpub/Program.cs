using NBitcoin;

var verticalSpace = new string('-', 80);

do
{   var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);

    var masterPrvKey = mnemonic.DeriveExtKey();
    var masterPubKey = masterPrvKey.Neuter();

    Console.WriteLine(verticalSpace);
    Console.WriteLine($"Mnemonic: {mnemonic}");
    Console.WriteLine($"Master private key hex: {masterPrvKey.PrivateKey.ToHex()}");
    Console.WriteLine($"Master private key: {masterPrvKey.ToString(Network.TestNet)}");
    Console.WriteLine($"Master public key hex: {masterPubKey.PubKey.ToHex()}");
    Console.WriteLine($"Master public key: {masterPubKey.ToString(Network.TestNet)}");

    Console.WriteLine(verticalSpace);
    Console.WriteLine("Press ESC to exit, any other key to generate a new key pair.");
} while (Console.ReadKey().Key != ConsoleKey.Escape);