using dotnetmud.Web.Database.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotnetmud.Web.Pages;

[AllowAnonymous]
public class IndexModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IWebHostEnvironment environment,
    ILogger<IndexModel> logger
    ) : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        logger.LogTrace("Home page requested.");

        // Determine if the user is authenticated, but not existing.  If in this state,
        // it's likely that the login is left over within Development mode, so just sign
        // the user out.
        if (environment.IsDevelopment() && (User.Identity?.IsAuthenticated ?? false))
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null)
            {
                logger.LogWarning("User {username} is authenticated, but not found in the database with the correct ID.  Signing out.", User.Identity?.Name);
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                await signInManager.SignOutAsync();
                return RedirectToPage("./Index");
            }
        }

        return Page();
    }
}
