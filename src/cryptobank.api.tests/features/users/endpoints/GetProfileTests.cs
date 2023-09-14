using cryptobank.api.features.users.endpoints.getProfile;
using cryptobank.api.tests.extensions;

namespace cryptobank.api.tests.features.users.endpoints;

public class GetProfileTests : IClassFixture<ApplicationFixture>
{
    private readonly ApplicationFixture _fixture;
    private readonly HttpClient _client;

    public GetProfileTests(ApplicationFixture fixture)
    {
        _fixture = fixture;

        _fixture.Authorize(_fixture.User);
        _client = _fixture.CreateClient();
    }

    [Fact]
    public async Task ShouldReturnUserProfile()
    {
        var res = await _client.GETAsync<EmptyRequest, ProfileModel>("/user/profile", new EmptyRequest());

        res.ShouldBeOk();
        res.Result.ShouldNotBeNull();
        res.Result.Email.ShouldBe(_fixture.User.Email);
        res.Result.Roles.ShouldBe(_fixture.User.Roles.Select(role => role.Name));
        res.Result.DateOfBirth.ShouldBe(_fixture.User.DateOfBirth);
        res.Result.DateOfRegistration.ShouldBe(_fixture.User.DateOfRegistration);
    }
}