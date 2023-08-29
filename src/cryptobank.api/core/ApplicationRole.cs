namespace cryptobank.api.core;

public static class ApplicationRole
{
    public const string User = "User";
    public const string Analyst = "Analyst";
    public const string Administrator = "Administrator";

    public const int UserRoleId = 1 << 0;
    public const int AnalystRoleId = 1 << 1;
    public const int AdministratorRoleId = 1 << 30;

    public static string[] All => new[] { User, Analyst, Administrator };
}