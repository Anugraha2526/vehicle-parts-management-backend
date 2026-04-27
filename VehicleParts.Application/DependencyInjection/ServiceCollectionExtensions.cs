using Microsoft.Extensions.DependencyInjection;
using VehicleParts.Application.Modules.AdminCore.Interfaces;
using VehicleParts.Application.Modules.AdminCore.Services;
using VehicleParts.Application.Modules.CustomerCRM.Interfaces;
using VehicleParts.Application.Modules.CustomerCRM.Services;
using VehicleParts.Application.Modules.CustomerPortal.Interfaces;
using VehicleParts.Application.Modules.CustomerPortal.Services;
using VehicleParts.Application.Modules.Finance.Interfaces;
using VehicleParts.Application.Modules.Finance.Services;
using VehicleParts.Application.Modules.Identity.Interfaces;
using VehicleParts.Application.Modules.Identity.Services;
using VehicleParts.Application.Modules.Sales.Interfaces;
using VehicleParts.Application.Modules.Sales.Services;

namespace VehicleParts.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAdminCoreService, AdminCoreService>();
        services.AddScoped<ICustomerCrmService, CustomerCrmService>();

        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ILowStockService, LowStockService>();

        services.AddScoped<ISalesService, SalesService>();
        services.AddScoped<ICustomerPortalService, CustomerPortalService>();

        return services;
    }
}
