using NBitcoin;

var verticalSpace = new string('-', 80);

do
{   var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);

    var masterPrvKey = mnemonic.DeriveExtKey();
    var masterPubKey = masterPrvKey.Neuter();

    var masterPpvKeyAsString = masterPrvKey.ToString(Network.TestNet);
    var masterPubKeyAsString = masterPubKey.ToString(Network.TestNet);

    Console.WriteLine(verticalSpace);
    Console.WriteLine($"Mnemonic: {mnemonic}");
    Console.WriteLine($"Master private key: {masterPpvKeyAsString}");
    Console.WriteLine($"Master public key: {masterPubKeyAsString}");
    Console.WriteLine(verticalSpace);
    Console.WriteLine("Press ESC to exit, any other key to generate a new key pair.");
} while (Console.ReadKey().Key != ConsoleKey.Escape);