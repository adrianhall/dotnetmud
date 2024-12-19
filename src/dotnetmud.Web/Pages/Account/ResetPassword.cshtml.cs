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
public class ResetPasswordModel(
    UserManager<ApplicationUser> userManager,
    ILogger<ResetPasswordModel> logger
    ) : PageModel
{
    /// <summary>
    /// The form model for the form submission.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; } = new();

    /// <summary>
    /// The model class for the form submission.
    /// </summary>
    public class InputModel
    {
        [Required, EmailAddress, Display(Name = "Email Address")]
        public string Username { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required, MinLength(32)]
        public string Code { get; set; } = string.Empty;
    }

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

        Input = new InputModel() { Username = user.UserName!, Code = code };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogTrace("OnPostAsync model={model}", Input.ToJsonString());

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await userManager.FindUserAsync(Input.Username);
        if (user is null)
        {
            logger.LogDebug("User {username} not found", Input.Username);
            return RedirectToPage("./Status", new { message = "Password reset failed.  Is the link still valid?" });
        }

        var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Input.Code));
        var result = await userManager.ResetPasswordAsync(user, token, Input.Password);
        if (result.Succeeded)
        {
            logger.LogDebug("User {username} password reset successfully", Input.Username);
            return RedirectToPage("./Status", new { message = "Password reset successfully.  You may now log in.", displayIndexLink = true });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return Page();
    }
}
