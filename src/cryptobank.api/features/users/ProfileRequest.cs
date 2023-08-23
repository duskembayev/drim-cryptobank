namespace cryptobank.api.features.users;

public class ProfileRequest : IRequest<ProfileResponse>
{
    public int UserId { get; set; }
}