using cryptobank.api.config;
using cryptobank.api.dal;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace cryptobank.api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<CryptoBankDbContext>(builder => builder.UseNpgsql(GetConnectionString()));
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }

    private string GetConnectionString()
    {
        var baseConnectionString = Configuration.GetConnectionString(ConfigConstants.DbConnectionStringName);
        var dbStringBuilder = new NpgsqlConnectionStringBuilder(baseConnectionString);
        var dbHost = Configuration.GetValue<string>(ConfigConstants.DbHostConfigKey);
        var dbPassword = Configuration.GetValue<string>(ConfigConstants.DbPasswordConfigKey);

        if (!string.IsNullOrEmpty(dbHost))
            dbStringBuilder.Host = dbHost;

        if (!string.IsNullOrEmpty(dbPassword))
            dbStringBuilder.Password = dbPassword;

        return dbStringBuilder.ConnectionString;
    }
}