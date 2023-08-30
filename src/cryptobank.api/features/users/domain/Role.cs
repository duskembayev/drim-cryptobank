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

        public static Role GetByName(string name)
        {
            if (User.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return User;

            if (Analyst.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return Analyst;

            if (Administrator.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return Administrator;

            throw new ArgumentOutOfRangeException(nameof(name));
        }
        
        public static bool Exists(string name)
        {
            if (User.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return true;

            if (Analyst.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return true;

            if (Administrator.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}