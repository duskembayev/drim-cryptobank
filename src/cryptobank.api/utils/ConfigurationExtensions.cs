using cryptobank.api.config;
using Npgsql;

namespace cryptobank.api.utils;

internal static class ConfigurationExtensions
{
    public static string GetNpgsqlConnectionString(this IConfiguration @this)
    {
        var baseConnectionString = @this.GetConnectionString(ConfigConstants.DbConnectionStringName);
        var dbStringBuilder = new NpgsqlConnectionStringBuilder(baseConnectionString);
        var dbHost = @this.GetValue<string>(ConfigConstants.DbHostConfigKey);
        var dbPassword = @this.GetValue<string>(ConfigConstants.DbPasswordConfigKey);

        if (!string.IsNullOrEmpty(dbHost))
            dbStringBuilder.Host = dbHost;

        if (!string.IsNullOrEmpty(dbPassword))
            dbStringBuilder.Password = dbPassword;

        return dbStringBuilder.ConnectionString;
    }
}