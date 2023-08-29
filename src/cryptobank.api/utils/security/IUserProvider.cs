namespace cryptobank.api.utils.security;

public interface IUserProvider
{
    public int Id { get; }
    public string Email { get; }
    public IEnumerable<string> Roles { get; }
}