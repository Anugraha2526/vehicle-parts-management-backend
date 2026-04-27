using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleParts.Application.Modules.AdminCore.Interfaces;
using VehicleParts.Application.Modules.Finance.Interfaces;
using VehicleParts.Infrastructure.Persistence;
using VehicleParts.Infrastructure.Repositories.AdminCore;
using VehicleParts.Infrastructure.Repositories.Finance;
using VehicleParts.Infrastructure.Security;

namespace VehicleParts.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing from configuration.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IStaffRepository, StaffRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();

        services.AddScoped<IPurchaseRepository, PurchaseRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<ILowStockRepository, LowStockRepository>();

        return services;
    }
}
