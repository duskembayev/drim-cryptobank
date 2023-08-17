using System.Text.RegularExpressions;
using Enhanced.DependencyInjection;

namespace cryptobank.api.utils.security;

[ContainerEntry(ServiceLifetime.Singleton, typeof(IEmailFormatValidator))]
internal partial class EmailFormatValidator : IEmailFormatValidator
{
    private static readonly Regex EmailRegex = BuildEmailRegex();
    
    public bool Validate(string email)
    {
        return EmailRegex.IsMatch(email);
    }

    [GeneratedRegex( @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline, "en-US")]
    private static partial Regex BuildEmailRegex();
}