using cryptobank.api.features.users.config;
using cryptobank.api.features.users.domain;
using cryptobank.api.features.users.services;
using cryptobank.api.utils.environment;
using Microsoft.IdentityModel.JsonWebTokens;

namespace cryptobank.api.tests.features.users.services;

public class AccessTokenProviderTests
{
    private const string ValidToken =
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjE2IiwiZW1haWwiOiJqb2huZG9lQGV4YW1wbGUuY29tIiwicm9sZXMiOlsiQW5hbHlzdCIsIlVzZXIiXSwiYXVkIjoiTXlBd2Vzb21lQXBwIiwiZXhwIjoxNjgzNDc1NTAwLCJpc3MiOiJNeUF3ZXNvbWVBdXRoUHJvdmlkZXIiLCJpYXQiOjE2ODM0NzUyMDAsIm5iZiI6MTY4MzQ3NTIwMH0.icFrivYpz-db8o3zA1jqXYRjtMytpmpoejOnVxf9BW8";

    private readonly AccessTokenProvider _accessTokenProvider;

    public AccessTokenProviderTests()
    {
        var options = new AccessTokenOptions
        {
            ClaimsIssuer = "MyAwesomeAuthProvider",
            Audience = "MyAwesomeApp",
            SecurityKey = "very-very-long-awesome-string-value-with-some-secret-words",
            Expiration = TimeSpan.FromMinutes(5)
        };

        var timeProvider = Substitute.For<ITimeProvider>();
        timeProvider.UtcNow.Returns(new DateTime(2023, 5, 7, 16, 0, 0, DateTimeKind.Utc));

        _accessTokenProvider = new AccessTokenProvider(
            new JsonWebTokenHandler(),
            timeProvider,
            new OptionsWrapper<AccessTokenOptions>(options));
    }

    [Fact]
    public void ShouldIssueJwtToken()
    {
        var token = _accessTokenProvider.Issue(new User
        {
            Id = 16,
            Email = "johndoe@example.com",
            Roles = { Role.Detached.Analyst, Role.Detached.User },
            DateOfBirth = new DateOnly(1986, 9, 30),
            DateOfRegistration = new DateTime(2020, 4, 4, 4, 31, 5, DateTimeKind.Utc),
            PasswordHash = ""
        });

        token.ShouldBe(ValidToken);
    }
}