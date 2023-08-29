using cryptobank.api.features.users.responses;

namespace cryptobank.api.features.users.requests;

public class ProfileRequest : IRequest<ProfileResponse>
{
    public int UserId { get; set; }
}