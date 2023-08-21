namespace cryptobank.api.features.users.domain;

public class Role
{
    public Role()
    {
    }

    public Role(Roles role)
    {
        if (role is Roles.None || !Enum.IsDefined(role))
            throw new ArgumentException("Invalid role", nameof(role));

        Id = (int) role;
        Name = role.ToString("G");
    }

    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}