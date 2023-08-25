namespace cryptobank.api.utils.security;

public interface IPasswordHashAlgorithm
{
    Task<string> HashAsync(string password);
    Task<bool> ValidateAsync(string delegatePassword, string formattedHash);
}