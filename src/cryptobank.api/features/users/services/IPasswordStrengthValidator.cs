namespace cryptobank.api.features.users.services;

public interface IPasswordStrengthValidator
{
    bool Validate(string password);
}