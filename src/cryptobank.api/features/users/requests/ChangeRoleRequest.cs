namespace cryptobank.api.features.users.requests;

public class ChangeRoleRequest : IRequest
{
    public int UserId { get; set; }
    public IReadOnlyCollection<string> Roles { get; set; } = Array.Empty<string>();
}