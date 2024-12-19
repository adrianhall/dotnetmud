using dotnetmud.Web.DataAnnotations;
using dotnetmud.Web.Database.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace dotnetmud.Web.Pages.Account;

[Authorize]
public class ProfileModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ILogger<ProfileModel> logger
    ) : PageModel
{
    /// <summary>
    /// The form model for the registration page.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; } = new();

    [TempData]
    public string StatusMessage { get; set; } = string.Empty;

    /// <summary>
    /// The input model for the registration page.
    /// </summary>
    public class InputModel
    {
        [Required, EmailAddress, Display(Name = "Email Address")]
        public string Username { get; set; } = string.Empty;

        [Required, DisplayName, Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        logger.LogTrace("OnGetAsync");

        var user = await userManager.GetUserAsync(User); 
        if (user is null)
        {
            logger.LogDebug("User {username} is authenticated, but not found in the database.", User.Identity?.Name);
            await signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }

        Input = new InputModel() { Username = user.UserName!, DisplayName = user.DisplayName! };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogTrace("OnPostAsync model={model}", Input.ToJsonString());

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            logger.LogDebug("User {username} is authenticated, but not found in the database.", User.Identity?.Name);
            await signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        user.DisplayName = Input.DisplayName;
        var result = await userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            logger.LogInformation("Updated user {username} with new display name {displayName}.", user.UserName, user.DisplayName);
            return RedirectToPage("/Index");
        }

        Input = new InputModel() { Username = user.UserName!, DisplayName = user.DisplayName! };
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }
}
