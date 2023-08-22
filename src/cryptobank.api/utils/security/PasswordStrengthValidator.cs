using System.Text.RegularExpressions;
using Enhanced.DependencyInjection;

namespace cryptobank.api.utils.security;

[ContainerEntry(ServiceLifetime.Singleton, typeof(IPasswordStrengthValidator))]
internal partial class PasswordStrengthValidator : IPasswordStrengthValidator
{
    private static readonly Regex ValidationRegex = BuildValidationRegex();

    public bool Validate(string password)
    {
        return ValidationRegex.IsMatch(password);
    }

    [GeneratedRegex(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        RegexOptions.Compiled | RegexOptions.Singleline)]
    private static partial Regex BuildValidationRegex();
}