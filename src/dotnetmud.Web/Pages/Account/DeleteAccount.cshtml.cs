using dotnetmud.Web.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotnetmud.Web.Pages.Account;

[Authorize]
public class DeleteAccountModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ILogger<DeleteAccountModel> logger
    ) : PageModel
{
    public async Task<IActionResult> OnPostAsync()
    {
        var userId = userManager.GetUserId(User);
        logger.LogInformation("OnPostAsync: User {userId} requesting account deletion.", userId);

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            logger.LogDebug("User {userId} is authenticated, but not found in the database.", userId);
            return NotFound($"Unable to load user with ID '{userId}'.");
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            logger.LogError("User {username} failed to delete themselves: {errors}", user.UserName, result.Errors);
            throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
        }

        await signInManager.SignOutAsync();
        logger.LogInformation("User '{username}' deleted themselves.", user.UserName);
        return RedirectToPage("./Status", new { Message = "Your account has been deleted.", DisplayIndexLink = true });
    }
}
