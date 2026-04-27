using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleParts.Application.Modules.Finance.Interfaces;
using VehicleParts.Infrastructure.Persistence;
using VehicleParts.Infrastructure.Repositories.Finance;

namespace VehicleParts.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing from configuration.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IPurchaseRepository, PurchaseRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<ILowStockRepository, LowStockRepository>();

        return services;
    }
}
