using dotnetmud.Web.Database.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace dotnetmud.Web.Pages.Account;

[Authorize]
public class ChangePasswordModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ILogger<ChangePasswordModel> logger
    ) : PageModel
{
    /// <summary>
    /// The form model for the registration page.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; } = new();

    /// <summary>
    /// The input model for the registration page.
    /// </summary>
    public class InputModel
    {
        [Required, DataType(DataType.Password), Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string OldPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogTrace("OnPostAsync model={model}", Input.ToJsonString());

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            logger.LogWarning("Invalid user state - signing out");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }

        var result = await userManager.ChangePasswordAsync(user, Input.OldPassword, Input.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        return RedirectToPage("./Status", new { Message = "Your password has been changed.", DisplayIndexLink = true });
    }
}
