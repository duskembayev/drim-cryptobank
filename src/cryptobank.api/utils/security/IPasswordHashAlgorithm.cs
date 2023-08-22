namespace cryptobank.api.utils.security;

public interface IPasswordHashAlgorithm
{
    byte[] GenerateSalt(int length);
    byte[] ComputeHash(string data, byte[] salt, int length);
    bool ValidateHash(string data, byte[] hash, byte[] salt);
}