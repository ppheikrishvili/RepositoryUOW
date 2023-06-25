using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RepositoryUOW;

namespace AddMigration;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        //var configuration = new ConfigurationBuilder()
        //    .SetBasePath(Directory.GetCurrentDirectory())
        //    .AddJsonFile("appsettings.json", true)
        //    .AddEnvironmentVariables()
        //    .Build();

        var builder = new DbContextOptionsBuilder();

        //var connectionString = configuration.GetConnectionString("DefaultConnection");

        var connectionString = @"Data Source=localhost;Initial Catalog=RepositoryUOWTest;" +
                               "User ID=sa;Password=123;Connection Timeout=30;";

        builder.UseSqlServer(connectionString,
            x => x.MigrationsAssembly(typeof(ApplicationDbContextFactory).Assembly.FullName));

        return new AppDbContext(builder.Options);
    }

}

