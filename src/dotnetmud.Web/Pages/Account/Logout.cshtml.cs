using dotnetmud.Web.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotnetmud.Web.Pages.Account;

[Authorize]
public class LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger) : PageModel
{
    public async Task<IActionResult> OnPost(string? returnUrl = null)
    {
        string username = User.Identity?.Name ?? "-";
        await signInManager.SignOutAsync();
        logger.LogInformation("User {name} logged out.", username);
        return returnUrl != null ? LocalRedirect(returnUrl) : RedirectToPage();
    }
}
