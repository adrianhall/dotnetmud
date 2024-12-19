using dotnetmud.Web.Database.Models;
using dotnetmud.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace dotnetmud.Web.Pages.Account;

[AllowAnonymous]
public class LoginModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IOptions<ServiceIdentityOptions> identityOptions,
    IWebHostEnvironment environment,
    ILogger<LoginModel> logger
    ) : PageModel
{
    /// <summary>
    /// The model for the login page.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; } = new();

    /// <summary>
    /// The list of external authentication providers.
    /// </summary>
    public IList<AuthenticationScheme> ExternalProviders { get; set; } = [];

    /// <summary>
    /// The return URL as provided on GET.
    /// </summary>
    public string ReturnUrl { get; set; } = string.Empty;

    /// <summary>
    /// A temporary error message to display.
    /// </summary>
    [TempData]
    public string? ErrorMessage { get; set; }

    private string InvalidPasswordMessage
        => environment.IsDevelopment() ? "Invalid password." : "Invalid username or password.";

    private string InvalidUserRecordMessage
        => environment.IsDevelopment() ? "Invalid user record." : "Invalid username or password.";

    public class InputModel
    {
        [Required, EmailAddress, Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        logger.LogTrace("OnGetAsync; returnUrl = {returnUrl}", returnUrl ?? "null");
        await SetRequiredPropertiesAsync(returnUrl);

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        // Ensure we are signed out to ensure a clean experience.
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        await signInManager.SignOutAsync();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        logger.LogTrace("OnPostAsync; returnUrl = {returnUrl}", returnUrl ?? "null");
        await SetRequiredPropertiesAsync(returnUrl);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        ApplicationUser? userRecord = await userManager.FindUserAsync(Input.Username);
        if (userRecord is null)
        {
            ModelState.AddModelError(string.Empty, InvalidUserRecordMessage);
            return Page();
        }

        var result = await signInManager.PasswordSignInAsync(userRecord, Input.Password, Input.RememberMe, lockoutOnFailure: identityOptions.Value.EnableLockout);
        if (result.Succeeded)
        {
            logger.LogInformation("User logged in.");
            return LocalRedirect(ReturnUrl);
        }

        if (result.RequiresTwoFactor)
        {
            ModelState.AddModelError(string.Empty, "Two-factor authentication is required, but not supported.");
            return Page();
        }

        if (result.IsLockedOut)
        {
            logger.LogWarning("User account {email} is locked out.", userRecord.Email);
            return RedirectToPage("./Lockout");
        }

        ModelState.AddModelError(string.Empty, InvalidPasswordMessage);
        return Page();
    }

    /// <summary>
    /// Sets the required properties for the page.
    /// </summary>
    /// <param name="returnUrl">The provided return URL</param>
    internal async Task SetRequiredPropertiesAsync(string? returnUrl)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        ExternalProviders = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }
}
