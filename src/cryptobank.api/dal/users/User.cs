namespace cryptobank.api.dal.users;

public class User
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public string PasswordSalt { get; init; } = string.Empty;
    public DateOnly DateOfBirth { get; init; } = DateOnly.MinValue;
    public DateTime DateOfRegistration { get; init; } = DateTime.UnixEpoch;
    public List<Role> Roles { get; } = new();
}