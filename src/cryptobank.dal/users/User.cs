namespace cryptobank.dal.users;

public class User
{
    private readonly string _email = string.Empty;

    public int Id { get; init; }

    public string Email
    {
        get => _email;
        init => _email = value.ToLowerInvariant();
    }

    public byte[] PasswordHash { get; init; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; init; } = Array.Empty<byte>();
    public DateOnly DateOfBirth { get; init; } = DateOnly.MinValue;
    public DateTime DateOfRegistration { get; init; } = DateTime.UnixEpoch;
    public RoleId Role
    {
        get => Roles.Aggregate(RoleId.None, (current, role) => current | (RoleId) role.Id);
        init
        {
            Roles.Clear();

            var roleIds = Enum.GetValues<RoleId>();

            foreach (var roleId in roleIds)
            {
                if (roleId is RoleId.None)
                    continue;

                if (value.HasFlag(roleId))
                    Roles.Add(new Role {Id = (int) value, Name = value.ToString("G")});
            }
        }
    }

    internal List<Role> Roles { get; } = new();
}