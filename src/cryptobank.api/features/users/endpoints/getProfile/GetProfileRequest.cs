using cryptobank.api.features.users.services;

namespace cryptobank.api.features.users.endpoints.getProfile;

public class GetProfileRequest : IRequest<ProfileModel>
{
    [FromClaim(AccessTokenConstants.ClaimsTypes.Id)]
    public int UserId { get; set; }
}