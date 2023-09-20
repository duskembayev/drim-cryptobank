namespace cryptobank.api.features.accounts.domain;

public class InternalTransfer
{
    public long Id { get; init; }

    public int SourceUserId { get; init; }
    public string SourceAccountId { get; init; } = string.Empty;
    public Currency SourceCurrency { get; init; }
    public decimal SourceAmount { get; init; }

    public int TargetUserId { get; init; }
    public string TargetAccountId { get; init; } = string.Empty;
    public Currency TargetCurrency { get; init; }
    public decimal TargetAmount { get; init; }

    public decimal ConversionRate { get; init; }
    public string Comment { get; init; } = string.Empty;
    public DateTime DateOfCreate { get; init; }
}