namespace cryptobank.api.features.users.domain;

public class User
{
    public int Id { get; init; }
    public string Email{ get; init; } = string.Empty;
    public byte[] PasswordHash { get; init; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; init; } = Array.Empty<byte>();
    public DateOnly DateOfBirth { get; init; } = DateOnly.MinValue;
    public DateTime DateOfRegistration { get; init; } = DateTime.UnixEpoch;
    public IList<Role> Roles { get; } = new List<Role>();
}