using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Sportradar.Calendar.Infrastructure.Persistence;

// design time factory so dotnet ef tools can build context without running app
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // read environment so we can load matching appsettings file
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        // migrations live in infrastructure, but configs sit in web project
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Sportradar.Calendar.Presentation.Web");

        // build configuration manually because ef tooling has no dependency injection
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // create options builder and hook to postgres connection string
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? configuration["DefaultConnection"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("The connection string 'DefaultConnection' was not found.");
        }

        optionsBuilder.UseNpgsql(connectionString);

        // return configured context so migrations command can use it immediately
        return new AppDbContext(optionsBuilder.Options);
    }
}