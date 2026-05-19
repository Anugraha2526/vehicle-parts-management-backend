using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.CustomerPortal.DTOs;

namespace VehicleParts.Application.Modules.CustomerPortal.Interfaces;

public interface ICustomerPortalService
{
    // Appointments
    Task<ServiceResult<AppointmentResponseDto>> BookAppointmentAsync(Guid customerId, BookAppointmentDto request, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<AppointmentResponseDto>>> GetAppointmentsAsync(Guid customerId, CancellationToken cancellationToken = default);

    // Reviews — any customer can read all published reviews
    Task<ServiceResult<ReviewResponseDto>> SubmitReviewAsync(Guid customerId, SubmitReviewDto request, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<ReviewResponseDto>>> GetReviewsAsync(CancellationToken cancellationToken = default);

    // Part requests
    Task<ServiceResult<PartRequestResponseDto>> RequestUnavailablePartAsync(Guid customerId, RequestPartDto request, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<PartRequestResponseDto>>> GetPartRequestsAsync(Guid customerId, CancellationToken cancellationToken = default);

    // Service history — customer's own sales invoices with summary stats
    Task<ServiceResult<ServiceHistoryDto>> GetServiceHistoryAsync(Guid customerId, CancellationToken cancellationToken = default);
}
