using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.Finance.DTOs;
using VehicleParts.Application.Modules.Finance.Interfaces;
using VehicleParts.Domain.Modules.Finance.Entities;

namespace VehicleParts.Application.Modules.Finance.Services;

public sealed class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;

    public PurchaseService(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }

    public async Task<ServiceResult<PurchaseInvoiceResponseDto>> CreatePurchaseInvoiceAsync(
        CreatePurchaseInvoiceDto request,
        CancellationToken cancellationToken = default)
    {
        if (request.Items.Count == 0)
        {
            return ServiceResult<PurchaseInvoiceResponseDto>.Fail("At least one purchase item is required.");
        }

        if (request.Items.Any(item => item.Quantity <= 0))
        {
            return ServiceResult<PurchaseInvoiceResponseDto>.Fail("Each purchase item quantity must be greater than zero.");
        }

        if (request.Items.Any(item => item.UnitCost < 0))
        {
            return ServiceResult<PurchaseInvoiceResponseDto>.Fail("Unit cost cannot be negative.");
        }

        var vendorExists = await _purchaseRepository.VendorExistsAsync(request.VendorId, cancellationToken);
        if (!vendorExists)
        {
            return ServiceResult<PurchaseInvoiceResponseDto>.Fail(
                $"Vendor not found: {request.VendorId}");
        }

        var partIds = request.Items.Select(item => item.PartId).Distinct().ToArray();
        var parts = await _purchaseRepository.GetPartsByIdsAsync(partIds, cancellationToken);
        var partMap = parts.ToDictionary(part => part.Id);

        var missingPartIds = partIds.Where(partId => !partMap.ContainsKey(partId)).ToArray();
        if (missingPartIds.Length > 0)
        {
            return ServiceResult<PurchaseInvoiceResponseDto>.Fail(
                $"Some parts were not found: {string.Join(", ", missingPartIds)}");
        }

        var purchasedAt = request.PurchasedAtUtc ?? DateTime.UtcNow;
        var invoice = new PurchaseInvoice
        {
            InvoiceNumber = GenerateInvoiceNumber(purchasedAt),
            VendorId = request.VendorId,
            PurchasedAtUtc = purchasedAt
        };

        foreach (var item in request.Items)
        {
            var part = partMap[item.PartId];
            part.QuantityInStock += item.Quantity;
            part.Touch();

            invoice.Items.Add(new PurchaseInvoiceItem
            {
                PartId = item.PartId,
                Quantity = item.Quantity,
                UnitCost = item.UnitCost
            });
        }

        invoice.RecalculateTotal();

        var createdInvoice = await _purchaseRepository.CreatePurchaseInvoiceAsync(invoice, cancellationToken);

        var response = new PurchaseInvoiceResponseDto
        {
            InvoiceId = createdInvoice.Id,
            InvoiceNumber = createdInvoice.InvoiceNumber,
            PurchasedAtUtc = createdInvoice.PurchasedAtUtc,
            TotalAmount = createdInvoice.TotalAmount,
            TotalItems = createdInvoice.Items.Sum(item => item.Quantity)
        };

        return ServiceResult<PurchaseInvoiceResponseDto>.Ok(response, "Purchase invoice created and stock updated.");
    }

    public async Task<ServiceResult<IReadOnlyList<PurchaseInvoiceResponseDto>>> GetPurchaseInvoicesAsync(
        CancellationToken cancellationToken = default)
    {
        var invoices = await _purchaseRepository.GetPurchaseInvoicesAsync(cancellationToken);
        var response = invoices.Select(MapInvoiceSummary).ToArray();
        return ServiceResult<IReadOnlyList<PurchaseInvoiceResponseDto>>.Ok(
            response,
            "Purchase invoices fetched.");
    }

    public async Task<ServiceResult<PurchaseInvoiceDetailResponseDto>> GetPurchaseInvoiceByIdAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _purchaseRepository.GetPurchaseInvoiceByIdAsync(invoiceId, cancellationToken);
        if (invoice is null)
        {
            return ServiceResult<PurchaseInvoiceDetailResponseDto>.Fail("Purchase invoice not found.");
        }

        var response = new PurchaseInvoiceDetailResponseDto
        {
            InvoiceId = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            VendorId = invoice.VendorId,
            PurchasedAtUtc = invoice.PurchasedAtUtc,
            TotalAmount = invoice.TotalAmount,
            TotalItems = invoice.Items.Sum(item => item.Quantity),
            Items = invoice.Items
                .Select(item => new PurchaseInvoiceLineItemResponseDto
                {
                    PartId = item.PartId,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    LineTotal = item.LineTotal
                })
                .ToArray()
        };

        return ServiceResult<PurchaseInvoiceDetailResponseDto>.Ok(response, "Purchase invoice fetched.");
    }

    private static PurchaseInvoiceResponseDto MapInvoiceSummary(PurchaseInvoice invoice)
    {
        return new PurchaseInvoiceResponseDto
        {
            InvoiceId = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            PurchasedAtUtc = invoice.PurchasedAtUtc,
            TotalAmount = invoice.TotalAmount,
            TotalItems = invoice.Items.Sum(item => item.Quantity)
        };
    }

    private static string GenerateInvoiceNumber(DateTime purchasedAtUtc)
    {
        return $"PINV-{purchasedAtUtc:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
    }
}
