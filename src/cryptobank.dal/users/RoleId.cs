namespace cryptobank.dal.users;

[Flags]
public enum RoleId
{
    None = 0,
    User = 1 << 0,
    Analyst = 1 << 1,
    Administrator = 1 << 30
}