namespace cryptobank.api.features.users.services;

public interface IPasswordHashAlgorithm
{
    Task<string> HashAsync(string password);
    Task<bool> ValidateAsync(string delegatePassword, string formattedHash);
}