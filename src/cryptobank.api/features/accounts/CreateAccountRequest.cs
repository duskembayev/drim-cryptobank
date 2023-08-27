using cryptobank.api.features.accounts.domain;
using cryptobank.api.utils;

namespace cryptobank.api.features.accounts;

public class CreateAccountRequest : IRequest<Account>
{
    public int UserId { get; set; }
    public Currency Currency { get; set; }
}