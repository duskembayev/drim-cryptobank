namespace cryptobank.api.features.users;

[Flags]
public enum Roles
{
    None = 0,
    User = 1 << 0,
    Analyst = 1 << 1,
    Administrator = 1 << 30
}