using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.CustomerPortal.Interfaces;

namespace VehicleParts.Application.Modules.CustomerPortal.Services;

public sealed class CustomerPortalService : ICustomerPortalService
{
    public Task<ServiceResult> BookAppointmentAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok("Appointment booking use case is wired."));

    public Task<ServiceResult> GetAppointmentsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok("Appointment list use case is wired."));

    public Task<ServiceResult> SubmitReviewAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok("Review submission use case is wired."));

    public Task<ServiceResult> GetReviewsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok("Review list use case is wired."));

    public Task<ServiceResult> RequestUnavailablePartAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok("Part request use case is wired."));

    public Task<ServiceResult> GetCustomerReportsAsync(string type, CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok($"Customer report use case is wired for '{type}'."));
}
