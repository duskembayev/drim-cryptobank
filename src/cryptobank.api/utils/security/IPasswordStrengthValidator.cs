namespace cryptobank.api.utils.security;

public interface IPasswordStrengthValidator
{
    bool Validate(string password);
}