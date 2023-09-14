using cryptobank.api.features.accounts.endpoints.reportOpenedDaily;
using cryptobank.api.tests.extensions;

namespace cryptobank.api.tests.features.accounts.endpoints;

public class ReportOpenedDailyTests : IClassFixture<ApplicationFixture>
{
    private readonly ApplicationFixture _fixture;
    private readonly HttpClient _client;

    public ReportOpenedDailyTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient(fixture.Analyst);
    }

    [Fact]
    public async Task ShouldReturnReport()
    {
        var result = await _client.GETAsync<ReportOpenedDailyRequest, IReadOnlyCollection<OpenedDailyModel>>(
            "/accounts/reports/openedDaily", new ReportOpenedDailyRequest
            {
                Start = new DateOnly(2020, 1, 1),
                End = new DateOnly(2020, 1, 5),
            });

        result.ShouldBeOk();
        result.Result.ShouldNotBeNull();
        result.Result.ShouldBe(new[]
        {
            new OpenedDailyModel(new DateOnly(2020, 1, 1), 3),
            new OpenedDailyModel(new DateOnly(2020, 1, 4), 1),
            new OpenedDailyModel(new DateOnly(2020, 1, 5), 2),
        });
    }

    [Fact]
    public async Task ShouldNotReturnReportWhenUserIsAdministrator()
    {
        using var adminClient = _fixture.CreateClient(_fixture.Administrator);

        var result = await adminClient.GETAsync<ReportOpenedDailyRequest, IReadOnlyCollection<OpenedDailyModel>>(
            "/accounts/reports/openedDaily", new ReportOpenedDailyRequest
            {
                Start = new DateOnly(2020, 1, 1),
                End = new DateOnly(2020, 1, 5),
            });
        
        result.ShouldBeWithStatus(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ShouldNotReturnReportWhenUserIsUser()
    {
        using var userClient = _fixture.CreateClient(_fixture.User);

        var result = await userClient.GETAsync<ReportOpenedDailyRequest, IReadOnlyCollection<OpenedDailyModel>>(
            "/accounts/reports/openedDaily", new ReportOpenedDailyRequest
            {
                Start = new DateOnly(2020, 1, 1),
                End = new DateOnly(2020, 1, 5),
            });
        
        result.ShouldBeWithStatus(HttpStatusCode.Forbidden);
    }
}