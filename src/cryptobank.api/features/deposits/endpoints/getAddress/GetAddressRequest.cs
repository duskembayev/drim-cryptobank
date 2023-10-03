using cryptobank.api.features.users.services;
using Microsoft.AspNetCore.Mvc;

namespace cryptobank.api.features.deposits.endpoints.getAddress;

public class GetAddressRequest : IRequest<DepositAddressModel>
{
    [FromClaim(AccessTokenConstants.ClaimsTypes.Id)]
    public int UserId { get; init; }

    [FromQuery] public required string AccountId { get; init; }

    public class Validator : AbstractValidator<GetAddressRequest>
    {
        public Validator()
        {
            RuleFor(x => x.AccountId)
                .NotEmpty()
                .WithErrorCode("deposits:get_address:account_id_empty");
        }
    }
}