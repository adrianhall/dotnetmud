using dotnetmud.Web.Database.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotnetmud.Web.Pages;

public class IndexModel(
    UserManager<ApplicationUser> userManager,
    ILogger<IndexModel> logger
    ) : PageModel
{
    public async Task OnGetAsync()
    {
        // Determine if the user is authenticated, but not existing.  If in this state,
        // it's likely that the login is left over within Development mode, so just sign
        // the user out.
        if (User.Identity?.IsAuthenticated == true)
        {
            string? username = User.Identity?.Name;
            if (username is not null)
            {
                var user = await userManager.FindUserAsync(username);
                if (user is null)
                {
                    logger.LogWarning("User {username} is authenticated, but not found in the database.  Signing out.", username);
                    await HttpContext.SignOutAsync();
                }
                // This is ok - the user is authenticated and found in the database.
            }
            else
            {
                logger.LogWarning("User is authenticated, but the username is null.  Signing out.");
                await HttpContext.SignOutAsync();
            }
        }
    }
}
