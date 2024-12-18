using dotnetmud.Web.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace dotnetmud.Web.Pages.Account;

public class ConfirmEmailModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ILogger<ConfirmEmailModel> logger
    ) : PageModel
{
    /// <summary>
    /// A status message to display.
    /// </summary>
    [TempData]
    public string? StatusMessage { get; set; }

    /// <summary>
    /// If true, we're going to display a link to the home page.
    /// </summary>
    public bool DisplayIndexLink { get; set; } = false;

    /// <summary>
    /// Callback - the user is sent a link via email and this is the entry point.
    /// </summary>
    public async Task<IActionResult> OnGetAsync(string userId, string code)
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
            StatusMessage = "Error: Invalid user ID.  The link may be no longer valid.";
            return Page();
        }

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Failed to decode code");
            StatusMessage = "Error: Invalid code.  The link is corrupted.";
            return Page();
        }

        var result = await userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            logger.LogInformation("User {email} confirmed account", user.Email);
            await signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Thank you for confirming your email.  You are now logged in.";
            DisplayIndexLink = true;
            return Page();
        }
        else
        {
            logger.LogDebug("Failed to confirm email for user {email}: {errors}", user.Email, result.Errors);
            StatusMessage = "Error confirmaing account.  The link may be no longer valid.";
            return Page();
        }
    }
}
