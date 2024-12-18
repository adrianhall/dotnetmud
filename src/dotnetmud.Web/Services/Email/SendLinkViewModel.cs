namespace dotnetmud.Web.Services.Email;

/// <summary>
/// The view model for the email templates for sending a link.
/// </summary>
public class SendLinkViewModel
{
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public required string Link { get; set; }
    public required string Subject { get; set; }
}
