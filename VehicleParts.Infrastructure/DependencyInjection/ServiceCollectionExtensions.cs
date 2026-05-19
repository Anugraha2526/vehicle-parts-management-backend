using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleParts.Application.Common.Interfaces;
using VehicleParts.Application.Modules.AdminCore.Interfaces;
using VehicleParts.Application.Modules.Finance.Interfaces;
using VehicleParts.Application.Modules.Sales.Interfaces;
using VehicleParts.Infrastructure.Persistence;
using VehicleParts.Infrastructure.Repositories.AdminCore;
using VehicleParts.Infrastructure.Repositories.Finance;
using VehicleParts.Infrastructure.Repositories.Sales;
using VehicleParts.Infrastructure.Security;
using VehicleParts.Infrastructure.Services;

namespace VehicleParts.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing from configuration.");

        // ✅ FIX: Force migrations to live in Infrastructure project
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                x => x.MigrationsAssembly("VehicleParts.Infrastructure")
            ));

        // Email (MailKit / SMTP)
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IEmailService, MailKitEmailService>();

        // Security
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        // Admin Core Repositories
        services.AddScoped<IStaffRepository, StaffRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IPartsRepository, PartsRepository>();

        // Finance Repositories
        services.AddScoped<IPurchaseRepository, PurchaseRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<ILowStockRepository, LowStockRepository>();

        // Sales Repositories
        services.AddScoped<ISalesRepository, SalesRepository>();

        return services;
    }
}