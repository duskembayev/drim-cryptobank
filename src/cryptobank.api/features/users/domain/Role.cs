using cryptobank.api.core;

namespace cryptobank.api.features.users.domain;

public class Role
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public static class Detached
    {
        public static readonly Role User = new()
        {
            Id = ApplicationRole.UserRoleId,
            Name = ApplicationRole.User
        };

        public static readonly Role Analyst = new()
        {
            Id = ApplicationRole.AnalystRoleId,
            Name = ApplicationRole.Analyst
        };

        public static readonly Role Administrator = new()
        {
            Id = ApplicationRole.AdministratorRoleId,
            Name = ApplicationRole.Administrator
        };
    }
}