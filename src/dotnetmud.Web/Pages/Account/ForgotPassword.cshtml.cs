using dotnetmud.Web.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace dotnetmud.Web.Pages.Account;

[AllowAnonymous]
public class ForgotPasswordModel(
    UserManager<ApplicationUser> userManager,
    IEmailSender<ApplicationUser> emailSender,
    ILogger<ForgotPasswordModel> logger
    ) : PageModel
{
    /// <summary>
    /// The model for the forgot password page.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; } = new();

    /// <summary>
    /// The model class for the form submission.
    /// </summary>
    public class InputModel
    {
        [Required, EmailAddress, Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogTrace("OnPostAsync input={model}", Input.ToJsonString());
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await userManager.FindUserAsync(Input.Username);
        if (user is null)
        {
            logger.LogDebug("Username {username} is not registered.", Input.Username);
            // Don't reveal that the user does not exist.
            return RedirectToPage("./ForgotPasswordConfirmation");
        }

        bool isConfirmed = await userManager.IsEmailConfirmedAsync(user);
        if (!isConfirmed)
        {
            logger.LogDebug("Username {username} is not confirmed.", Input.Username);
            // Don't reveal that the user does not exist.
            return RedirectToPage("./ForgotPasswordConfirmation");
        }

        string resetLink = await CreatePasswordResetLinkAsync(user);
        await emailSender.SendPasswordResetLinkAsync(user, user.Email!, resetLink);
        return RedirectToPage("./ForgotPasswordConfirmation");
    }

    /// <summary>
    /// Creates a confirmation link for the user.
    /// </summary>
    /// <param name="user">The user record.</param>
    /// <returns>The confirmation link.</returns>
    internal async Task<string> CreatePasswordResetLinkAsync(ApplicationUser user)
    {
        var userId = await userManager.GetUserIdAsync(user);
        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        return Url.Page("/Account/ResetPassword", pageHandler: null, values: new { userId, code }, protocol: Request.Scheme)
            ?? throw new ApplicationException("Cannot create URL for Password Reset");
    }
}
