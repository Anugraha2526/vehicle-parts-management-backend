using Microsoft.Extensions.Logging;
using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.Sales.DTOs;
using VehicleParts.Application.Modules.Sales.Interfaces;
using VehicleParts.Domain.Modules.Sales.Entities;

namespace VehicleParts.Application.Modules.Sales.Services;

public sealed class SalesService : ISalesService
{
    private readonly ISalesRepository _salesRepository;
    private readonly ILogger<SalesService> _logger;

    public SalesService(ISalesRepository salesRepository, ILogger<SalesService> logger)
    {
        _salesRepository = salesRepository;
        _logger = logger;
    }

    public async Task<ServiceResult<SalesInvoiceResponseDto>> CreateSalesInvoiceAsync(
        CreateSalesInvoiceDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // --- Validate input ---
            if (request.Items.Count == 0)
                return ServiceResult<SalesInvoiceResponseDto>.Fail("At least one item is required.");

            if (request.Items.Any(i => i.Quantity <= 0))
                return ServiceResult<SalesInvoiceResponseDto>.Fail("Each item quantity must be greater than zero.");

            // --- Resolve parts from DB ---
            var partIds = request.Items.Select(i => i.PartId).Distinct().ToArray();
            var parts = await _salesRepository.GetPartsByIdsAsync(partIds, cancellationToken);
            var partMap = parts.ToDictionary(p => p.Id);

            var missingIds = partIds.Where(id => !partMap.ContainsKey(id)).ToArray();
            if (missingIds.Length > 0)
                return ServiceResult<SalesInvoiceResponseDto>.Fail(
                    $"Parts not found: {string.Join(", ", missingIds)}");

            // --- Check sufficient stock ---
            foreach (var item in request.Items)
            {
                var part = partMap[item.PartId];
                if (part.StockQuantity < item.Quantity)
                    return ServiceResult<SalesInvoiceResponseDto>.Fail(
                        $"Insufficient stock for '{part.Name}'. Available: {part.StockQuantity}, Requested: {item.Quantity}.");
            }

            // --- Build invoice ---
            var soldAt = DateTime.UtcNow;
            var invoice = new SalesInvoice
            {
                InvoiceNumber = GenerateInvoiceNumber(soldAt),
                CustomerId = request.CustomerId,
                StaffId = request.StaffId,
                SoldAtUtc = soldAt
            };

            foreach (var item in request.Items)
            {
                var part = partMap[item.PartId];
                invoice.Items.Add(new SalesInvoiceItem
                {
                    SalesInvoiceId = invoice.Id,
                    PartId = item.PartId,
                    PartName = part.Name,
                    Quantity = item.Quantity,
                    UnitPrice = part.UnitPrice
                });
            }

            // RecalculateTotal applies the 10% loyalty discount when SubTotal > 5000
            invoice.RecalculateTotal();

            var created = await _salesRepository.CreateSalesInvoiceAsync(invoice, cancellationToken);

            _logger.LogInformation(
                "Sales invoice {InvoiceNumber} created. Total: {Total}, DiscountApplied: {Discount}",
                created.InvoiceNumber, created.TotalAmount, created.LoyaltyDiscountApplied);

            var response = MapToResponseDto(created);
            var message = created.LoyaltyDiscountApplied
                ? $"Invoice created with 10% loyalty discount applied. You saved {created.DiscountAmount:C}."
                : "Sales invoice created successfully.";

            return ServiceResult<SalesInvoiceResponseDto>.Ok(response, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sales invoice for customer {CustomerId}", request.CustomerId);
            return ServiceResult<SalesInvoiceResponseDto>.Fail("An unexpected error occurred while creating the invoice.");
        }
    }

    public async Task<ServiceResult> SendInvoiceEmailAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        var invoice = await _salesRepository.GetSalesInvoiceByIdAsync(invoiceId, cancellationToken);
        if (invoice is null)
            return ServiceResult.Fail($"Invoice with ID '{invoiceId}' was not found.");

        // Email sending is implemented in the email-invoice branch (F11)
        _logger.LogInformation("Email send requested for invoice {InvoiceId}", invoiceId);
        return ServiceResult.Ok($"Email queued for invoice {invoice.InvoiceNumber}.");
    }

    private static string GenerateInvoiceNumber(DateTime soldAt)
        => $"SINV-{soldAt:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";

    public async Task<ServiceResult<List<SalesInvoiceResponseDto>>> GetRecentInvoicesAsync(int limit = 10, CancellationToken cancellationToken = default)
    {
        var invoices = await _salesRepository.GetRecentInvoicesAsync(limit, cancellationToken);
        var dtos = invoices.Select(inv => MapToResponseDto(inv)).ToList();
        return ServiceResult<List<SalesInvoiceResponseDto>>.Ok(dtos, "Recent invoices fetched.");
    }

    private static SalesInvoiceResponseDto MapToResponseDto(SalesInvoice invoice)
    {
        return new SalesInvoiceResponseDto
        {
            InvoiceId = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerId = invoice.CustomerId,
            CustomerName = invoice.Customer?.FullName ?? "Unknown",
            StaffId = invoice.StaffId,
            SoldAtUtc = invoice.CreatedAtUtc,
            SubTotal = invoice.SubTotal,
            LoyaltyDiscountApplied = invoice.LoyaltyDiscountApplied,
            DiscountAmount = invoice.DiscountAmount,
            TotalAmount = invoice.TotalAmount,
            Items = invoice.Items.Select(i => new SalesInvoiceItemResponseDto
            {
                PartId = i.PartId,
                PartName = i.PartName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                SubTotal = i.SubTotal
            }).ToList()
        };
    }
}

