using cryptobank.api.features.accounts.domain;
using cryptobank.api.utils;

namespace cryptobank.api.features.accounts;

public class CreateAccountRequest : IRequest<OperationResponse<string>>
{
    public Currency Currency { get; set; }
}