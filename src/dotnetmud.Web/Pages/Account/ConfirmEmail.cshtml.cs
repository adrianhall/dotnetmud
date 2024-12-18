using dotnetmud.Web.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace dotnetmud.Web.Pages.Account;

[AllowAnonymous]
public class ConfirmEmailModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ILogger<ConfirmEmailModel> logger
    ) : PageModel
{
    /// <summary>
    /// Callback - the user is sent a link via email and this is the entry point.
    /// </summary>
    public async Task<IActionResult> OnGetAsync(string? userId, string? code)
    {
        logger.LogTrace("OnGetAsync userId={userId}, code={code}", userId, code);

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
        {
            logger.LogDebug("Error: Invalid parameters provided");
            return RedirectToPage("/Index");
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            logger.LogDebug("User ID {userId} not found", userId);
            return RedirectToPage("./Status", new { message = "Error: Invalid user ID.  The link may be no longer valid." });
        }

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Failed to decode code");
            return RedirectToPage("./Status", new { message = "Error: Invalid code.  The link is corrupted." });
        }

        var result = await userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            logger.LogInformation("User {email} confirmed account", user.Email);
            await signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToPage("./Status", new { message = "Thank you for confirming your email.  You are now logged in.", displayIndexLink = true });
        }
        else
        {
            logger.LogDebug("Failed to confirm email for user {email}: {errors}", user.Email, result.Errors);
            return RedirectToPage("./Status", new { message = "Error confirmaing account.  The link may be no longer valid." });
        }
    }
}
