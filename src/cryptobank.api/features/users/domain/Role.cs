using System.Diagnostics.CodeAnalysis;

namespace cryptobank.api.features.users.domain;

public class Role
{
    public const string User = "User";
    public const string Analyst = "Analyst";
    public const string Administrator = "Administrator";

    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public static class Detached
    {
        public static readonly Role User = new()
        {
            Id = 1 << 0,
            Name = Role.User
        };

        public static readonly Role Analyst = new()
        {
            Id = 1 << 1,
            Name = Role.Analyst
        };

        public static readonly Role Administrator = new()
        {
            Id = 1 << 30,
            Name = Role.Administrator
        };

        public static readonly ImmutableArray<Role> All = ImmutableArray.Create(User, Analyst, Administrator); 

        public static Role GetByName(string name)
            => All.Single(role => role.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public static bool Exists(string name)
            => All.Any(role => role.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}