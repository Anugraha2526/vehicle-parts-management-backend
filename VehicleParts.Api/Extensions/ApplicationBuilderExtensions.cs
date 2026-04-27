using VehicleParts.Api.Middleware;

namespace VehicleParts.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
