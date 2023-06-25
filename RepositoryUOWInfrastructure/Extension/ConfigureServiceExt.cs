
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RepositoryUOW;
using Serilog;

namespace RepositoryUOWInfrastructure.Extension;

public static class ConfigureServiceExt
{
    public static void AddDbContext(this IServiceCollection serviceCollection,
        IConfiguration configuration, IConfigurationRoot configRoot)
    {
        serviceCollection.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ConnectionString") ?? configRoot["ConnectionStrings:ConnectionString"]
                , b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
    }

    public static void AddSerLog(this WebApplicationBuilder applicationBuilder)
    {
        var logger = new LoggerConfiguration().ReadFrom.Configuration(applicationBuilder.Configuration)
            .Enrich.FromLogContext().CreateLogger();
        applicationBuilder.Logging.ClearProviders();
        applicationBuilder.Logging.AddSerilog(logger);
    }
}