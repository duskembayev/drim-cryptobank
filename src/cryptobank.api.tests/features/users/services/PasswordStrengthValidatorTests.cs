using cryptobank.api.features.users.services;

namespace cryptobank.api.tests.features.users.services;

[Collection(UsersCollection.Name)]
public class PasswordStrengthValidatorTests
{
    private readonly PasswordStrengthValidator _validator = new();

    [Theory]
    [InlineData("P@ssw0rd")]
    [InlineData("P@ssw0r!")]
    [InlineData("12345Qq!")]
    [InlineData("abdefgh1!Password")]
    public void ShouldPass(string password)
    {
        Assert.True(_validator.Validate(password));
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("123456Qq")]
    [InlineData("abcdefghijklmn")]
    [InlineData("MyAwes0mePassword")]
    [InlineData("0987654321!")]
    [InlineData("awesomep@$sw0rd")]
    public void ShouldFail(string password)
    {
        Assert.False(_validator.Validate(password));
    }
}