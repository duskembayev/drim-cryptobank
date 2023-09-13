using cryptobank.api.features.users.responses;
using cryptobank.api.tests.extensions;
using FastEndpoints;
using Shouldly;

namespace cryptobank.api.tests.features.users;

public class ProfileTests : IClassFixture<ApplicationFixture>
{
    private readonly ApplicationFixture _fixture;
    private readonly HttpClient _client;

    public ProfileTests(ApplicationFixture fixture)
    {
        _fixture = fixture;

        _fixture.Authorize(_fixture.User);
        _client = _fixture.CreateClient();
    }

    [Fact]
    public async Task ShouldReturnUserProfile()
    {
        var res = await _client.GETAsync<EmptyRequest, ProfileResponse>("/user/profile", new EmptyRequest());

        res.ShouldBeOk();
        res.Result.ShouldNotBeNull();
        res.Result.Email.ShouldBe(_fixture.User.Email);
        res.Result.Roles.ShouldBe(_fixture.User.Roles.Select(role => role.Name));
        res.Result.DateOfBirth.ShouldBe(_fixture.User.DateOfBirth);
        res.Result.DateOfRegistration.ShouldBe(_fixture.User.DateOfRegistration);
    }
}