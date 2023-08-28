using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.users.services;
using Microsoft.AspNetCore.Mvc;

namespace cryptobank.api.features.accounts;

public class CreateAccountRequest : IRequest<Account>
{
    [FromClaim(AccessTokenConstants.ClaimsTypes.Id)]
    public int UserId { get; set; }

    [FromQuery] public Currency Currency { get; set; } = Currency.USD;
}