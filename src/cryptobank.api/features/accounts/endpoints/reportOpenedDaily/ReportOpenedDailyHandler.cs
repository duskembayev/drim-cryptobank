namespace cryptobank.api.features.accounts.endpoints.reportOpenedDaily;

public class ReportOpenedDailyHandler : IRequestHandler<ReportOpenedDailyRequest, IReadOnlyCollection<OpenedDailyModel>>
{
    private readonly CryptoBankDbContext _dbContext;

    public ReportOpenedDailyHandler(CryptoBankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<OpenedDailyModel>> Handle(
        ReportOpenedDailyRequest request,
        CancellationToken cancellationToken)
    {
        var startInclusive = request.Start.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endExclusive = request.End.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        return await _dbContext.Accounts
            .Where(a => a.DateOfOpening >= startInclusive && a.DateOfOpening < endExclusive)
            .GroupBy(a => a.DateOfOpening.Date)
            .Select(g => new OpenedDailyModel(DateOnly.FromDateTime(g.Key), g.Count()))
            .ToListAsync(cancellationToken);
    }
}
