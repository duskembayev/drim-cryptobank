namespace cryptobank.api.features.users.services;

public interface IEmailFormatValidator
{
    bool Validate(string email);
}