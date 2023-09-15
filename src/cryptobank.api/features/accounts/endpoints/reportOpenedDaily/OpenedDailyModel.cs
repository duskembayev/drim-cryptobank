namespace cryptobank.api.features.accounts.endpoints.reportOpenedDaily;

public record OpenedDailyModel(DateOnly Date, int Count);