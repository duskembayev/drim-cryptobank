using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.users.services;

namespace cryptobank.api.features.accounts.endpoints.openAccount;

public class OpenAccountRequest : IRequest<string>
{
    [FromClaim(AccessTokenConstants.ClaimsTypes.Id)]
    public int UserId { get; set; }

    [FromQueryParams]
    public Currency Currency { get; set; } = Currency.USD;

    public class Validator : AbstractValidator<OpenAccountRequest>
    {
        public Validator()
        {
            RuleFor(request => request.Currency).ValidEnumValue();
        }
    }
}