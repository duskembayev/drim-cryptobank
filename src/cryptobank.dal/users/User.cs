namespace cryptobank.dal.users;

public class User
{
    private readonly List<Role> _roles = new();

    public int Id { get; init; }
    public string Email{ get; init; } = string.Empty;
    public byte[] PasswordHash { get; init; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; init; } = Array.Empty<byte>();
    public DateOnly DateOfBirth { get; init; } = DateOnly.MinValue;
    public DateTime DateOfRegistration { get; init; } = DateTime.UnixEpoch;
    public RoleId Role
    {
        get => Roles.Aggregate(RoleId.None, (current, role) => current | (RoleId) role.Id);
        init
        {
            _roles.Clear();

            var roleIds = Enum.GetValues<RoleId>();

            foreach (var roleId in roleIds)
            {
                if (roleId is RoleId.None)
                    continue;

                if (value.HasFlag(roleId))
                    _roles.Add(new Role {Id = (int) value, Name = value.ToString("G")});
            }
        }
    }

    public IReadOnlyList<Role> Roles => _roles;
}