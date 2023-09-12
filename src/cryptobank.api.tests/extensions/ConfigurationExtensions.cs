using Microsoft.Extensions.Configuration;
using Npgsql;

namespace cryptobank.api.tests.extensions;

public static class ConfigurationExtensions
{
    public static string GetConnectionStringWithRndDatabase(this IConfiguration @this, string name = "postgres")
    {
        var originalConnectionString = @this.GetConnectionString(name);
        var dbName = "db_" + Guid.NewGuid().ToString("N");

        var npgConnStringBuilder = new NpgsqlConnectionStringBuilder(originalConnectionString)
        {
            Database = dbName
        };

        return npgConnStringBuilder.ConnectionString;
    }
}