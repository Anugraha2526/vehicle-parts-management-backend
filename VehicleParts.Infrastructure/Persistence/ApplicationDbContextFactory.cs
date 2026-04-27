using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VehicleParts.Infrastructure.Persistence;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var connectionString = ResolveConnectionString();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static string ResolveConnectionString()
    {
        var overrideConnection = Environment.GetEnvironmentVariable("VEHICLEPARTS_CONNECTION");
        if (!string.IsNullOrWhiteSpace(overrideConnection))
        {
            return overrideConnection;
        }

        var apiBasePath = ResolveApiBasePath();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiBasePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection not found. Set it in VehicleParts.Api appsettings or VEHICLEPARTS_CONNECTION environment variable.");
    }

    private static string ResolveApiBasePath()
    {
        var current = Directory.GetCurrentDirectory();

        var direct = Path.Combine(current, "VehicleParts.Api");
        if (Directory.Exists(direct))
        {
            return direct;
        }

        var parent = Path.GetFullPath(Path.Combine(current, "..", "VehicleParts.Api"));
        if (Directory.Exists(parent))
        {
            return parent;
        }

        return current;
    }
}
