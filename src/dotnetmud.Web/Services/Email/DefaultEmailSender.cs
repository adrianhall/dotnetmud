using dotnetmud.Web.Database.Models;
using dotnetmud.Web.Models;
using dotnetmud.Web.Services.Email;
using Microsoft.AspNetCore.Identity;

namespace dotnetmud.Web.Services;

public class DefaultEmailSender(
    IEmailProvider emailProvider,
    IViewRenderingService templateRenderer,
    ILogger<DefaultEmailSender> logger
    ) : IEmailSender<ApplicationUser>
{
    private const string TemplateFolder = "/Views/EmailTemplates";

    /// <inheritdoc />
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        logger.LogInformation("SendConfirmationLink: email={email},link={confirmationLink}", email, confirmationLink);
        return SendLinkAsync(user, email, confirmationLink, "ConfirmationLink", "Confirm your account!");
    }

    /// <inheritdoc />
    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        logger.LogInformation("SendPasswordResetLink: email={email},link={resetLink}", email, resetLink);
        return SendLinkAsync(user, email, resetLink, "PasswordResetLink", "Your password reset request");
    }

    internal async Task SendLinkAsync(ApplicationUser user, string email, string link, string templateName, string subject)
    {
        logger.LogInformation("SendLink: email={email},link={link},template={templateName},subject={subject}", email, link, templateName, subject);
        SendLinkViewModel viewModel = new() { DisplayName = user.DisplayName, Email = email, Link = link, Subject = subject };
        EmailMessage message = new()
        {
            Recipients = [new EmailAddress(email) { DisplayName = user.DisplayName }],
            Subject = subject,
            TextContent = await templateRenderer.RenderViewAsync($"{TemplateFolder}/{templateName}.TEXT.cshtml", viewModel),
            HtmlContent = await templateRenderer.RenderViewAsync($"{TemplateFolder}/{templateName}.HTML.cshtml", viewModel)
        };
        EmailResult result = await emailProvider.SendEmailAsync(message);
        if (result.Succeeded)
        {
            logger.LogInformation("Email sent successfully to {email}", email);
        }
        else
        {
            logger.LogError("Email send failed to {email}: {messages}", email, result.Messages);
        }
    }
}
