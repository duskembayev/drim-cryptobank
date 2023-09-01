using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.users.services;

namespace cryptobank.api.features.accounts.requests;

public class CreateAccountRequest : IRequest<string>
{
    [FromClaim(AccessTokenConstants.ClaimsTypes.Id)]
    public int UserId { get; set; }

    [FromQueryParams]
    public Currency Currency { get; set; } = Currency.USD;

    public class Validator : AbstractValidator<CreateAccountRequest>
    {
        public Validator()
        {
            RuleFor(request => request.Currency).ValidEnumValue();
            RuleFor(request => request.UserId).ValidUserId();
        }
    }
}