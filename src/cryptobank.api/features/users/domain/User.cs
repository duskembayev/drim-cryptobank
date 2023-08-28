using cryptobank.api.core;

namespace cryptobank.api.features.users.domain;

public class User
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public DateOnly DateOfBirth { get; init; } = DateOnly.MinValue;
    public DateTime DateOfRegistration { get; init; } = DateTime.UnixEpoch;
    public IList<Role> Roles { get; } = new List<Role>(ApplicationRole.All.Length);
}