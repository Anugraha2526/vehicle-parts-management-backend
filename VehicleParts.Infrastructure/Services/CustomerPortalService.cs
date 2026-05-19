using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.CustomerPortal.DTOs;
using VehicleParts.Application.Modules.CustomerPortal.Interfaces;
using VehicleParts.Domain.Modules.CustomerPortal.Entities;
using VehicleParts.Infrastructure.Persistence;

namespace VehicleParts.Infrastructure.Services;

public sealed class CustomerPortalService : ICustomerPortalService
{
    private readonly ApplicationDbContext _db;

    public CustomerPortalService(ApplicationDbContext db)
    {
        _db = db;
    }

    // ─── Appointments ────────────────────────────────────────────────

    public async Task<ServiceResult<AppointmentResponseDto>> BookAppointmentAsync(
        Guid customerId,
        BookAppointmentDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ServiceType))
            return ServiceResult<AppointmentResponseDto>.Fail("Service type is required.");

        if (request.AppointmentAtUtc <= DateTime.UtcNow)
            return ServiceResult<AppointmentResponseDto>.Fail("Appointment must be scheduled for a future date and time.");

        var appointment = new Appointment
        {
            CustomerId = customerId,
            AppointmentAtUtc = request.AppointmentAtUtc,
            ServiceType = request.ServiceType.Trim(),
            Status = "Pending",
            Notes = request.Notes.Trim()
        };

        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync(cancellationToken);

        return ServiceResult<AppointmentResponseDto>.Ok(MapAppointment(appointment), "Appointment booked successfully.");
    }

    public async Task<ServiceResult<List<AppointmentResponseDto>>> GetAppointmentsAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var appointments = await _db.Appointments
            .Where(a => a.CustomerId == customerId)
            .OrderByDescending(a => a.AppointmentAtUtc)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return ServiceResult<List<AppointmentResponseDto>>.Ok(appointments.Select(MapAppointment).ToList());
    }

    // ─── Reviews ─────────────────────────────────────────────────────

    public async Task<ServiceResult<ReviewResponseDto>> SubmitReviewAsync(
        Guid customerId,
        SubmitReviewDto request,
        CancellationToken cancellationToken = default)
    {
        if (request.Rating < 1 || request.Rating > 5)
            return ServiceResult<ReviewResponseDto>.Fail("Rating must be between 1 and 5.");

        if (string.IsNullOrWhiteSpace(request.ServiceType))
            return ServiceResult<ReviewResponseDto>.Fail("Service type is required.");

        if (string.IsNullOrWhiteSpace(request.Comment))
            return ServiceResult<ReviewResponseDto>.Fail("Comment is required.");

        var review = new ServiceReview
        {
            CustomerId = customerId,
            Rating = request.Rating,
            ServiceType = request.ServiceType.Trim(),
            Comment = request.Comment.Trim()
        };

        _db.ServiceReviews.Add(review);
        await _db.SaveChangesAsync(cancellationToken);

        var customerName = await _db.Users
            .Where(u => u.Id == customerId)
            .Select(u => u.FullName)
            .FirstOrDefaultAsync(cancellationToken);

        return ServiceResult<ReviewResponseDto>.Ok(MapReview(review, customerName ?? "Customer"), "Review submitted. Thank you for your feedback!");
    }

    public async Task<ServiceResult<List<ReviewResponseDto>>> GetReviewsAsync(
        CancellationToken cancellationToken = default)
    {
        // Join reviews with Users to surface the customer name
        var rows = await _db.ServiceReviews
            .Join(_db.Users,
                r => r.CustomerId,
                u => u.Id,
                (r, u) => new { Review = r, u.FullName })
            .OrderByDescending(x => x.Review.CreatedAtUtc)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dtos = rows.Select(x => MapReview(x.Review, x.FullName)).ToList();
        return ServiceResult<List<ReviewResponseDto>>.Ok(dtos);
    }

    // ─── Part Requests ────────────────────────────────────────────────

    public async Task<ServiceResult<PartRequestResponseDto>> RequestUnavailablePartAsync(
        Guid customerId,
        RequestPartDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RequestedPartName))
            return ServiceResult<PartRequestResponseDto>.Fail("Part name is required.");

        // Block duplicate pending requests for the same part by the same customer
        var duplicate = await _db.PartRequests.AnyAsync(
            pr => pr.CustomerId == customerId
                  && pr.RequestedPartName == request.RequestedPartName.Trim()
                  && pr.Status == "Pending",
            cancellationToken);

        if (duplicate)
            return ServiceResult<PartRequestResponseDto>.Fail("You already have a pending request for this part.");

        var partRequest = new PartRequest
        {
            CustomerId = customerId,
            RequestedPartName = request.RequestedPartName.Trim(),
            Description = request.Description.Trim(),
            Status = "Pending"
        };

        _db.PartRequests.Add(partRequest);
        await _db.SaveChangesAsync(cancellationToken);

        return ServiceResult<PartRequestResponseDto>.Ok(MapPartRequest(partRequest), "Your part request has been submitted. We will notify you once it becomes available.");
    }

    public async Task<ServiceResult<List<PartRequestResponseDto>>> GetPartRequestsAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var requests = await _db.PartRequests
            .Where(pr => pr.CustomerId == customerId)
            .OrderByDescending(pr => pr.CreatedAtUtc)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return ServiceResult<List<PartRequestResponseDto>>.Ok(requests.Select(MapPartRequest).ToList());
    }

    // ─── Service History ─────────────────────────────────────────────

    public async Task<ServiceResult<ServiceHistoryDto>> GetServiceHistoryAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var invoices = await _db.SalesInvoices
            .Include(i => i.Items)
            .Where(i => i.CustomerId == customerId)
            .OrderByDescending(i => i.SoldAtUtc)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dto = new ServiceHistoryDto
        {
            TotalSpent = invoices.Where(i => i.IsPaid).Sum(i => i.TotalAmount),
            InvoiceCount = invoices.Count,
            PaidCount = invoices.Count(i => i.IsPaid),
            UnpaidCount = invoices.Count(i => !i.IsPaid),
            TotalSaved = invoices.Sum(i => i.DiscountAmount),
            LastPurchaseDate = invoices.FirstOrDefault()?.SoldAtUtc,
            Invoices = invoices.Select(MapInvoice).ToList()
        };

        return ServiceResult<ServiceHistoryDto>.Ok(dto);
    }

    // ─── Private mappers ─────────────────────────────────────────────

    private static AppointmentResponseDto MapAppointment(Appointment a) => new()
    {
        Id = a.Id,
        CustomerId = a.CustomerId,
        AppointmentAtUtc = a.AppointmentAtUtc,
        ServiceType = a.ServiceType,
        Status = a.Status,
        Notes = a.Notes,
        CreatedAtUtc = a.CreatedAtUtc
    };

    private static ReviewResponseDto MapReview(ServiceReview r, string customerName) => new()
    {
        Id = r.Id,
        CustomerId = r.CustomerId,
        CustomerName = customerName,
        Rating = r.Rating,
        ServiceType = r.ServiceType,
        Comment = r.Comment,
        CreatedAtUtc = r.CreatedAtUtc
    };

    private static PartRequestResponseDto MapPartRequest(PartRequest pr) => new()
    {
        Id = pr.Id,
        CustomerId = pr.CustomerId,
        RequestedPartName = pr.RequestedPartName,
        Description = pr.Description,
        Status = pr.Status,
        CreatedAtUtc = pr.CreatedAtUtc
    };

    private static InvoiceResponseDto MapInvoice(VehicleParts.Domain.Modules.Sales.Entities.SalesInvoice inv) => new()
    {
        Id = inv.Id,
        InvoiceNumber = inv.InvoiceNumber,
        SoldAtUtc = inv.SoldAtUtc,
        SubTotal = inv.SubTotal,
        LoyaltyDiscountApplied = inv.LoyaltyDiscountApplied,
        DiscountAmount = inv.DiscountAmount,
        TotalAmount = inv.TotalAmount,
        IsPaid = inv.IsPaid,
        PaidAtUtc = inv.PaidAtUtc,
        Items = inv.Items.Select(item => new InvoiceItemDto
        {
            PartName = item.PartName,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            SubTotal = item.SubTotal
        }).ToList()
    };
}
