using cryptobank.api.features.users.config;
using cryptobank.api.features.users.services;
using cryptobank.api.utils.environment;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;

namespace cryptobank.api.tests.features.users.services;

public class PasswordHashAlgorithmTests
{
    private const string ValidPassword = "MyAwes0m3P@$sw0rd";

    private const string ValidFormattedHash =
        "$argon2id$v=1$m=1024,t=2,p=2$cEAioguBVj8=$LSc34XKFTpyObydZ4MlatG2VBxvqbPEKbMCI7Hrcp1A=";

    private readonly PasswordHashAlgorithm _passwordHashAlgorithm;

    public PasswordHashAlgorithmTests()
    {
        var options = new PasswordHashOptions
        {
            SaltSize = 8,
            DegreeOfParallelism = 2,
            MemorySize = 1024,
            HashSize = 32,
            Iterations = 2
        };

        var rndBytesGenerator = Substitute.For<IRndBytesGenerator>();

        rndBytesGenerator.GetBytes(options.SaltSize)
            .Returns(new byte[] { 112, 64, 34, 162, 11, 129, 86, 63 });

        rndBytesGenerator.GetBytes(Arg.Is<int>(i => i != options.SaltSize))
            .Throws(_ => new InvalidOperationException());

        _passwordHashAlgorithm =
            new PasswordHashAlgorithm(rndBytesGenerator, new OptionsWrapper<PasswordHashOptions>(options));
    }

    [Fact]
    public async Task ShouldHashPassword()
    {
        var hash = await _passwordHashAlgorithm.HashAsync(ValidPassword);
        hash.ShouldBe(ValidFormattedHash);
    }

    [Fact]
    public async Task ShouldValidatePassword()
    {
        var result = await _passwordHashAlgorithm.ValidateAsync(ValidPassword, ValidFormattedHash);

        result.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldReturnFalseOnValidatePasswordWhenInvalidPassword()
    {
        var result = await _passwordHashAlgorithm.ValidateAsync("someAn0therP@$sword", ValidFormattedHash);

        result.ShouldBeFalse();
    }

    [Fact]
    public async Task ShouldValidateWithFormattedHashOptions()
    {
        const string legacyFormattedHash = "$argon2id$v=1$m=512,t=1,p=5$cEAioguB$cJzs5r6qowL3RGdEqxfH+Q==";
        var result = await _passwordHashAlgorithm.ValidateAsync(ValidPassword, legacyFormattedHash);

        result.ShouldBeTrue();
    }

    [Theory]
    [InlineData("$argon2id$v=1$t=1,p=5$cEAioguB$cJzs5r6qowL3RGdEqxfH+Q==")]
    [InlineData("$argon2id$v=1$m=512,p=5$cEAioguB$cJzs5r6qowL3RGdEqxfH+Q==")]
    [InlineData("$argon2id$v=1$m=512,t=1$cEAioguB$cJzs5r6qowL3RGdEqxfH+Q==")]
    [InlineData("$argon2id$v=1$m=512,t=1,k=5$cEAioguB$cJzs5r6qowL3RGdEqxfH+Q==")]
    [InlineData("$argon2id$v=1$m=512,j=1,p=5$cEAioguB$cJzs5r6qowL3RGdEqxfH+Q==")]
    [InlineData("$argon2id$v=1$n=512,t=1,p=5$cEAioguB$cJzs5r6qowL3RGdEqxfH+Q==")]
    [InlineData("$argon2id$v=1$m=512,t=1,p=5,t=3$cEAioguB$cJzs5r6qowL3RGdEqxfH+Q==")]
    [InlineData("$argon2id$v=1$m=512,t=1,p=5$t=3$cEAioguB$cJzs5r6qowL3RGdEqxfH+Q==")]
    public async Task ShouldThrowExceptionWhenHashHasInvalidFormat(string invalidHash)
    {
        await Assert.ThrowsAsync<FormatException>(
            () => _passwordHashAlgorithm.ValidateAsync(ValidPassword, invalidHash));
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenHashIsNotSupported()
    {
        await Assert.ThrowsAsync<NotSupportedException>(
            () => _passwordHashAlgorithm.ValidateAsync(ValidPassword,
                "$argon2$v=1$m=512,t=1,p=5$cEAioguB$cJzs5r6qowL3RGdEqxfH+Q=="));
    }
}