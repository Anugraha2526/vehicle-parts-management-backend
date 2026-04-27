using VehicleParts.Application.Common.Models;

namespace VehicleParts.Application.Modules.CustomerPortal.Interfaces;

public interface ICustomerPortalService
{
    Task<ServiceResult> BookAppointmentAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> GetAppointmentsAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> SubmitReviewAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> GetReviewsAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> RequestUnavailablePartAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> GetCustomerReportsAsync(string type, CancellationToken cancellationToken = default);
}
