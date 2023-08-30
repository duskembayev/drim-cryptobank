using cryptobank.api.features.users.responses;
using cryptobank.api.features.users.services;

namespace cryptobank.api.features.users.requests;

public class ProfileRequest : IRequest<ProfileResponse>
{
    [FromClaim(AccessTokenConstants.ClaimsTypes.Id)]
    public int UserId { get; set; }

    public class Validator : AbstractValidator<ProfileRequest>
    {
        public Validator()
        {
            RuleFor(request => request.UserId).ValidUserId();
        }
    }
}