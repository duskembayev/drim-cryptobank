using cryptobank.api.features.users.config;
using cryptobank.api.features.users.requests;
using cryptobank.api.features.users.services;
using cryptobank.api.utils.environment;

namespace cryptobank.api.tests.features.users;

public class RegisterUserRequestValidatorTests : IClassFixture<DbFixture>
{
    private readonly ITimeProvider _timeProvider;
    private readonly RegisterUserRequest.Validator _validator;
    private readonly IPasswordStrengthValidator _passwordStrengthValidator;

    public RegisterUserRequestValidatorTests(DbFixture fixture)
    {
        _timeProvider = Substitute.For<ITimeProvider>();
        _timeProvider.UtcNow.Returns(new DateTime(2023, 6, 5, 11, 2, 37, DateTimeKind.Utc));
        _timeProvider.Today.Returns(new DateOnly(2023, 6, 5));

        _passwordStrengthValidator = Substitute.For<IPasswordStrengthValidator>();
        _passwordStrengthValidator
            .Validate(Arg.Any<string>())
            .Returns(true);

        _validator = new RegisterUserRequest.Validator(
            _passwordStrengthValidator,
            _timeProvider,
            fixture.DbContext,
            new OptionsWrapper<RegisterUserOptions>(new RegisterUserOptions()));
    }

    [Theory]
    [InlineData("email")]
    [InlineData("email@")]
    [InlineData("@domain")]
    [InlineData("domain.com")]
    public async Task ShouldValidateEmail(string email)
    {
        var result = await _validator.TestValidateAsync(new RegisterUserRequest
        {
            Email = email,
            Password = "P@s$w0rd",
            DateOfBirth = new DateOnly(2011, 5, 6)
        });

        result
            .ShouldHaveValidationErrorFor(request => request.Email)
            .WithErrorCode("users:register:email_invalid");
    }

    [Theory]
    [InlineData("fry@futurama.com")]
    [InlineData("bender@futurama.com")]
    public async Task ShouldValidateEmailNotUsed(string email)
    {
        var result = await _validator.TestValidateAsync(new RegisterUserRequest
        {
            Email = email,
            Password = "P@s$w0rd",
            DateOfBirth = new DateOnly(1986, 5, 6)
        });

        result
            .ShouldHaveValidationErrorFor(request => request.Email)
            .WithErrorCode("users:register:user_exists");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(14)]
    [InlineData(15)]
    public async Task ShouldValidateDayOfBirthIsInvalid(int age)
    {
        var result = await _validator.TestValidateAsync(new RegisterUserRequest
            {
                Email = "newUser@example.com",
                Password = "newUserP@ssw0rd",
                DateOfBirth = _timeProvider.Today.AddYears(-age)
            }
        );

        result
            .ShouldHaveValidationErrorFor(request => request.DateOfBirth)
            .WithErrorCode("users:register:too_young");
    }

    [Fact]
    public async Task ShouldValidatePasswordIsWeak()
    {
        _passwordStrengthValidator
            .Validate("123456")
            .Returns(false);

        var result = await _validator.TestValidateAsync(new RegisterUserRequest
            {
                Email = "newUser@example.com",
                Password = "123456",
                DateOfBirth = new DateOnly(1986, 5, 6)
            }
        );

        result
            .ShouldHaveValidationErrorFor(request => request.Password)
            .WithErrorCode("users:register:password_weak");
    }
}