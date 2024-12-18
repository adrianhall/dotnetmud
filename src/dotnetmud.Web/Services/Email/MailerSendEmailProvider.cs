using dotnetmud.Web.Models;

namespace dotnetmud.Web.Services;

/// <summary>
/// An email provider for the MailerSend service.
/// </summary>
/// <remarks>
/// TODO: Need to implement this class.
/// </remarks>
public class MailerSendEmailProvider : IEmailProvider
{
    /// <inheritdoc />
    public Task<EmailResult> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
