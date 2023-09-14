using cryptobank.api.features.accounts.endpoints.reportOpenedDaily;
using cryptobank.api.tests.extensions;

namespace cryptobank.api.tests.features.accounts.endpoints;

public class ReportOpenedDailyTests : IClassFixture<ApplicationFixture>
{
    private readonly HttpClient _client;

    public ReportOpenedDailyTests(ApplicationFixture fixture)
    {
        fixture.Authorize(fixture.Analyst);
        _client = fixture.CreateClient();
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
}