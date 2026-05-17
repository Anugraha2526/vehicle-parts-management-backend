namespace VehicleParts.Application.Common.Interfaces;

/// <summary>
/// Abstraction for sending transactional emails.
/// Implemented by MailKitEmailService in the Infrastructure layer.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a plain-text or HTML email to a single recipient.
    /// </summary>
    Task SendAsync(
        string toEmail,
        string toName,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default);
}
