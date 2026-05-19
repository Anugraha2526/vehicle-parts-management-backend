using Microsoft.Extensions.Logging;
using VehicleParts.Application.Common.Interfaces;
using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.Sales.DTOs;
using VehicleParts.Application.Modules.Sales.Interfaces;
using VehicleParts.Domain.Modules.Sales.Entities;

namespace VehicleParts.Application.Modules.Sales.Services;

public sealed class SalesService : ISalesService
{
    private readonly ISalesRepository _salesRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<SalesService> _logger;

    public SalesService(
        ISalesRepository salesRepository,
        IEmailService emailService,
        ILogger<SalesService> logger)
    {
        _salesRepository = salesRepository;
        _emailService    = emailService;
        _logger          = logger;
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
            var parts   = await _salesRepository.GetPartsByIdsAsync(partIds, cancellationToken);
            var partMap = parts.ToDictionary(p => p.Id);

            var missingIds = partIds.Where(id => !partMap.ContainsKey(id)).ToArray();
            if (missingIds.Length > 0)
                return ServiceResult<SalesInvoiceResponseDto>.Fail(
                    $"Parts not found: {string.Join(", ", missingIds)}");

            // --- Check sufficient stock ---
            foreach (var item in request.Items)
            {
                var part = partMap[item.PartId];
                if (part.QuantityInStock < item.Quantity)
                    return ServiceResult<SalesInvoiceResponseDto>.Fail(
                        $"Insufficient stock for '{part.PartName}'. Available: {part.QuantityInStock}, Requested: {item.Quantity}.");
            }

            // --- Build invoice ---
            var soldAt  = DateTime.UtcNow;
            var invoice = new SalesInvoice
            {
                InvoiceNumber = GenerateInvoiceNumber(soldAt),
                CustomerId    = request.CustomerId,
                StaffId       = request.StaffId,
                SoldAtUtc     = soldAt
            };

            foreach (var item in request.Items)
            {
                var part = partMap[item.PartId];
                invoice.Items.Add(new SalesInvoiceItem
                {
                    SalesInvoiceId = invoice.Id,
                    PartId = item.PartId,
                    PartName = part.PartName,
                    Quantity = item.Quantity,
                    UnitPrice = part.SellingPrice
                });
            }

            // RecalculateTotal applies the 10% loyalty discount when SubTotal > 5000
            invoice.RecalculateTotal();

            var created = await _salesRepository.CreateSalesInvoiceAsync(invoice, cancellationToken);

            _logger.LogInformation(
                "Sales invoice {InvoiceNumber} created. Total: {Total}, DiscountApplied: {Discount}",
                created.InvoiceNumber, created.TotalAmount, created.LoyaltyDiscountApplied);

            var response = MapToResponseDto(created);
            var message  = created.LoyaltyDiscountApplied
                ? $"Invoice created with 10% loyalty discount applied. You saved {created.DiscountAmount:C}."
                : "Sales invoice created successfully.";

            return ServiceResult<SalesInvoiceResponseDto>.Ok(response, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sales invoice for customer {CustomerId}", request.CustomerId);
            return ServiceResult<SalesInvoiceResponseDto>.Fail($"Exception: {ex.Message} | Inner: {ex.InnerException?.Message}");
        }
    }

    public async Task<ServiceResult> SendInvoiceEmailAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        var invoice = await _salesRepository.GetSalesInvoiceByIdAsync(invoiceId, cancellationToken);
        if (invoice is null)
            return ServiceResult.Fail($"Invoice with ID '{invoiceId}' was not found.");

        var customer = invoice.Customer;
        if (customer is null)
            return ServiceResult.Fail("Customer information could not be found for this invoice.");

        // Customer must have an email address stored
        var toEmail = customer.Email;
        if (string.IsNullOrWhiteSpace(toEmail))
            return ServiceResult.Fail("The customer does not have an email address on file.");

        try
        {
            var subject  = $"Your ChitoSpare Invoice – {invoice.InvoiceNumber}";
            var htmlBody = BuildInvoiceEmailHtml(invoice);

            await _emailService.SendAsync(toEmail, customer.FullName, subject, htmlBody, cancellationToken);

            _logger.LogInformation(
                "Invoice email sent for {InvoiceNumber} to {Email}", invoice.InvoiceNumber, toEmail);

            return ServiceResult.Ok($"Invoice {invoice.InvoiceNumber} sent to {toEmail}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email for invoice {InvoiceId}", invoiceId);
            return ServiceResult.Fail("Email could not be sent. Please check the SMTP configuration.");
        }
    }

    public async Task<ServiceResult<List<SalesInvoiceResponseDto>>> GetRecentInvoicesAsync(
        int limit = 10, CancellationToken cancellationToken = default)
    {
        var invoices = await _salesRepository.GetRecentInvoicesAsync(limit, cancellationToken);
        var dtos     = invoices.Select(MapToResponseDto).ToList();
        return ServiceResult<List<SalesInvoiceResponseDto>>.Ok(dtos, "Recent invoices fetched.");
    }

    // ──────────────────────────── helpers ────────────────────────────

    public async Task<ServiceResult<List<SalesInvoiceResponseDto>>> GetUnpaidInvoicesAsync(
        int olderThanMonths = 1, CancellationToken cancellationToken = default)
    {
        var invoices = await _salesRepository.GetUnpaidInvoicesAsync(olderThanMonths, cancellationToken);
        var dtos     = invoices.Select(MapToResponseDto).ToList();
        return ServiceResult<List<SalesInvoiceResponseDto>>.Ok(dtos, "Unpaid overdue invoices fetched.");
    }

    public async Task<ServiceResult> SendDueRemindersAsync(
        int olderThanMonths = 1, Guid? specificInvoiceId = null, CancellationToken cancellationToken = default)
    {
        var invoices = await _salesRepository.GetUnpaidInvoicesAsync(olderThanMonths, cancellationToken);
        
        if (specificInvoiceId.HasValue)
        {
            invoices = invoices.Where(i => i.Id == specificInvoiceId.Value).ToList();
        }

        if (!invoices.Any())
            return ServiceResult.Fail("No overdue invoices found for reminding.");

        int successCount = 0;
        foreach(var invoice in invoices)
        {
            var customer = invoice.Customer;
            if (customer == null || string.IsNullOrWhiteSpace(customer.Email)) continue;

            var subject = $"Payment Reminder: ChitoSpare Invoice {invoice.InvoiceNumber}";
            var ageInDays = (DateTime.UtcNow - invoice.SoldAtUtc).Days;
            
            // Per user request, plain text email.
            var textBody = $@"Dear {customer.FullName},

This is a friendly reminder from ChitoSpare regarding your unpaid invoice {invoice.InvoiceNumber}.
The invoice was issued on {invoice.SoldAtUtc.ToString("dd MMM yyyy")} and is currently {ageInDays} days overdue.

Outstanding Balance: Rs. {invoice.TotalAmount:N2}

Please arrange to settle this balance at your earliest convenience to avoid any service interruptions.

Thank you,
The ChitoSpare Team";

            try
            {
                await _emailService.SendAsync(customer.Email, customer.FullName, subject, textBody, cancellationToken);
                successCount++;
                _logger.LogInformation("Sent plain text reminder to {Email} for overdue invoice {InvoiceNumber}", customer.Email, invoice.InvoiceNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reminder to {Email}", customer.Email);
            }
        }

        return ServiceResult.Ok($"Successfully sent {successCount} reminders.");
    }

    public async Task<ServiceResult> MarkInvoiceAsPaidAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        var result = await _salesRepository.MarkInvoiceAsPaidAsync(invoiceId, cancellationToken);
        if (!result)
            return ServiceResult.Fail($"Invoice {invoiceId} could not be marked as paid. It may not exist or is already paid.");

        return ServiceResult.Ok("Invoice successfully marked as paid.");
    }

    public async Task<ServiceResult> MarkInvoiceAsUnpaidAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        var result = await _salesRepository.MarkInvoiceAsUnpaidAsync(invoiceId, cancellationToken);
        if (!result)
            return ServiceResult.Fail($"Invoice {invoiceId} could not be marked as unpaid. It may not exist or is already unpaid.");

        return ServiceResult.Ok("Invoice successfully marked as unpaid.");
    }

    private static string GenerateInvoiceNumber(DateTime soldAt)
        => $"SINV-{soldAt:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";

    private static SalesInvoiceResponseDto MapToResponseDto(SalesInvoice invoice)
    {
        return new SalesInvoiceResponseDto
        {
            InvoiceId              = invoice.Id,
            InvoiceNumber          = invoice.InvoiceNumber,
            CustomerId             = invoice.CustomerId,
            CustomerName           = invoice.Customer?.FullName ?? "Unknown",
            StaffId                = invoice.StaffId,
            SoldAtUtc              = invoice.CreatedAtUtc,
            SubTotal               = invoice.SubTotal,
            LoyaltyDiscountApplied = invoice.LoyaltyDiscountApplied,
            DiscountAmount         = invoice.DiscountAmount,
            TotalAmount            = invoice.TotalAmount,
            IsPaid                 = invoice.IsPaid,
            Items = invoice.Items.Select(i => new SalesInvoiceItemResponseDto
            {
                PartId    = i.PartId,
                PartName  = i.PartName,
                Quantity  = i.Quantity,
                UnitPrice = i.UnitPrice,
                SubTotal  = i.SubTotal
            }).ToList()
        };
    }

    /// <summary>Builds a clean HTML invoice email body.</summary>
    private static string BuildInvoiceEmailHtml(SalesInvoice invoice)
    {
        var rows = string.Join("", invoice.Items.Select(i => $"""
            <tr>
              <td style="padding:8px 12px;border-bottom:1px solid #eee;">{i.PartName}</td>
              <td style="padding:8px 12px;border-bottom:1px solid #eee;text-align:center;">{i.Quantity}</td>
              <td style="padding:8px 12px;border-bottom:1px solid #eee;text-align:right;">Rs. {i.UnitPrice:N2}</td>
              <td style="padding:8px 12px;border-bottom:1px solid #eee;text-align:right;">Rs. {i.SubTotal:N2}</td>
            </tr>
            """));

        var discountRow = invoice.LoyaltyDiscountApplied
            ? $"""
              <tr>
                <td colspan="3" style="padding:8px 12px;text-align:right;color:#16a34a;">Loyalty Discount (10%):</td>
                <td style="padding:8px 12px;text-align:right;color:#16a34a;">- Rs. {invoice.DiscountAmount:N2}</td>
              </tr>
              """
            : "";

        var customerName = invoice.Customer?.FullName ?? "Valued Customer";
        var date         = invoice.SoldAtUtc.ToString("dd MMM yyyy, hh:mm tt") + " UTC";

        return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head><meta charset="UTF-8"><meta name="viewport" content="width=device-width,initial-scale=1"></head>
            <body style="margin:0;padding:0;background:#f4f4f5;font-family:'Segoe UI',Arial,sans-serif;">
              <table width="100%" cellpadding="0" cellspacing="0" style="background:#f4f4f5;padding:40px 0;">
                <tr><td align="center">
                  <table width="600" cellpadding="0" cellspacing="0" style="background:#fff;border-radius:12px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,0.06);">

                    <!-- Header -->
                    <tr>
                      <td style="background:#1e293b;padding:28px 32px;">
                        <h1 style="margin:0;color:#fff;font-size:22px;letter-spacing:0.5px;">ChitoSpare</h1>
                        <p style="margin:4px 0 0;color:#94a3b8;font-size:13px;">Vehicle Parts Management</p>
                      </td>
                    </tr>

                    <!-- Body -->
                    <tr>
                      <td style="padding:32px;">
                        <p style="margin:0 0 8px;font-size:15px;color:#374151;">Dear <strong>{customerName}</strong>,</p>
                        <p style="margin:0 0 24px;font-size:14px;color:#6b7280;">
                          Thank you for your purchase. Please find your invoice details below.
                        </p>

                        <!-- Invoice Meta -->
                        <table width="100%" cellpadding="0" cellspacing="0" style="margin-bottom:24px;">
                          <tr>
                            <td style="font-size:13px;color:#6b7280;">Invoice Number</td>
                            <td style="font-size:13px;color:#111827;text-align:right;font-weight:600;">{invoice.InvoiceNumber}</td>
                          </tr>
                          <tr>
                            <td style="font-size:13px;color:#6b7280;padding-top:6px;">Date</td>
                            <td style="font-size:13px;color:#111827;text-align:right;padding-top:6px;">{date}</td>
                          </tr>
                        </table>

                        <!-- Items Table -->
                        <table width="100%" cellpadding="0" cellspacing="0" style="border:1px solid #e5e7eb;border-radius:8px;overflow:hidden;margin-bottom:16px;">
                          <thead>
                            <tr style="background:#f9fafb;">
                              <th style="padding:10px 12px;text-align:left;font-size:12px;color:#6b7280;font-weight:600;text-transform:uppercase;">Part</th>
                              <th style="padding:10px 12px;text-align:center;font-size:12px;color:#6b7280;font-weight:600;text-transform:uppercase;">Qty</th>
                              <th style="padding:10px 12px;text-align:right;font-size:12px;color:#6b7280;font-weight:600;text-transform:uppercase;">Unit Price</th>
                              <th style="padding:10px 12px;text-align:right;font-size:12px;color:#6b7280;font-weight:600;text-transform:uppercase;">Subtotal</th>
                            </tr>
                          </thead>
                          <tbody>{rows}</tbody>
                        </table>

                        <!-- Totals -->
                        <table width="100%" cellpadding="0" cellspacing="0" style="margin-bottom:32px;">
                          <tr>
                            <td colspan="3" style="padding:8px 12px;text-align:right;color:#6b7280;font-size:14px;">Subtotal:</td>
                            <td style="padding:8px 12px;text-align:right;font-size:14px;">Rs. {invoice.SubTotal:N2}</td>
                          </tr>
                          {discountRow}
                          <tr style="background:#f9fafb;">
                            <td colspan="3" style="padding:12px;text-align:right;font-weight:700;font-size:16px;color:#111827;">Total Amount:</td>
                            <td style="padding:12px;text-align:right;font-weight:700;font-size:16px;color:#1e293b;">Rs. {invoice.TotalAmount:N2}</td>
                          </tr>
                        </table>

                        <p style="margin:0;font-size:13px;color:#9ca3af;">
                          If you have any questions, please contact us at support@chitospare.com.
                        </p>
                      </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                      <td style="background:#f9fafb;padding:20px 32px;text-align:center;">
                        <p style="margin:0;font-size:12px;color:#9ca3af;">© {DateTime.UtcNow.Year} ChitoSpare. All rights reserved.</p>
                      </td>
                    </tr>

                  </table>
                </td></tr>
              </table>
            </body>
            </html>
            """;
    }
}
