using dotnetmud.Web.Models;

namespace dotnetmud.Web.Services;

/// <summary>
/// A description of the email service provider.
/// </summary>
public interface IEmailProvider
{
    /// <summary>
    /// Sends an email message asynchronously.
    /// </summary>
    /// <param name="message">The message to be sent.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe.</param>
    /// <returns>The results of the operation.</returns>
    Task<EmailResult> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
