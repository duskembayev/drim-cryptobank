using cryptobank.api.features.users.endpoints.getProfile;
using cryptobank.api.tests.extensions;

namespace cryptobank.api.tests.features.users.endpoints;

[Collection(UsersCollection.Name)]
public class GetProfileTests
{
    private readonly ApplicationFixture _fixture;
    private readonly HttpClient _client;

    public GetProfileTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.HttpClient.CreateClient(_fixture.Setup.User);
    }

    [Fact]
    public async Task ShouldReturnUserProfile()
    {
        var res = await _client.GETAsync<EmptyRequest, ProfileModel>("/user/profile", new EmptyRequest());

        res.ShouldBeOk();
        res.Result.ShouldNotBeNull();
        res.Result.Email.ShouldBe(_fixture.Setup.User.Email);
        res.Result.Roles.ShouldBe(_fixture.Setup.User.Roles.Select(role => role.Name));
        res.Result.DateOfBirth.ShouldBe(_fixture.Setup.User.DateOfBirth);
        res.Result.DateOfRegistration.ShouldBe(_fixture.Setup.User.DateOfRegistration);
    }
}