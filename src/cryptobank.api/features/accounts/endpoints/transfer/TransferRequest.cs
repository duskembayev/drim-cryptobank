using cryptobank.api.features.users.services;

namespace cryptobank.api.features.accounts.endpoints.transfer;

public class TransferRequest : IRequest<TransferModel>
{
    [FromClaim(AccessTokenConstants.ClaimsTypes.Id)]
    public int UserId { get; set; }

    public string SourceAccountId { get; set; } = string.Empty;
    public string TargetAccountId { get; set; } = string.Empty;

    public decimal Amount { get; set; }
    public string Comment { get; set; } = string.Empty;
}