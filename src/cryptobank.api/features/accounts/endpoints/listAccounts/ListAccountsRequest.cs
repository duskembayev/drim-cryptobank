using cryptobank.api.features.users.services;

namespace cryptobank.api.features.accounts.endpoints.listAccounts;

public class ListAccountsRequest : IRequest<IReadOnlyCollection<AccountModel>>
{
    [FromClaim(AccessTokenConstants.ClaimsTypes.Id)]
    public int UserId { get; set; }

    public class Validator : AbstractValidator<ListAccountsRequest>
    {
        public Validator()
        {
            RuleFor(request => request.UserId).ValidUserId();
        }
    }
}