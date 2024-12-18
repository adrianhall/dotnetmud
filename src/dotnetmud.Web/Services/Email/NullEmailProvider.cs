using dotnetmud.Web.Models;

namespace dotnetmud.Web.Services;

/// <summary>
/// A dummy email provider to ensure we don't send any email.
/// </summary>
public class NullEmailProvider : IEmailProvider
{
    /// <inheritdoc />
    public Task<EmailResult> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
        => Task.FromResult(new EmailResult { Succeeded = true, ResultCode = 200 });
}
