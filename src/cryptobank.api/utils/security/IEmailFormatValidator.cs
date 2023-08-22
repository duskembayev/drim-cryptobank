namespace cryptobank.api.utils.security;

public interface IEmailFormatValidator
{
    bool Validate(string email);
}