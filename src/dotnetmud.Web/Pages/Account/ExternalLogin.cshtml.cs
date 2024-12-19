using dotnetmud.Web.DataAnnotations;
using dotnetmud.Web.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;

namespace dotnetmud.Web.Pages.Account;

[AllowAnonymous]
public class ExternalLoginModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IEmailSender<ApplicationUser> emailSender,
    ILogger<ExternalLoginModel> logger
    ) : PageModel
{
    /// <summary>
    /// The input model for form submission.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; } = new();

    /// <summary>
    /// The display name for the OAuth provider.
    /// </summary>
    public string ProviderDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The requested return URL.
    /// </summary>
    public string ReturnUrl { get; set; } = string.Empty;

    /// <summary>
    /// The error message - passed back to the login page.
    /// </summary>
    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// The model class for the form submission.
    /// </summary>
    public class InputModel
    {
        [Required, EmailAddress, Display(Name = "Email Address")]
        public string Username { get; set; } = string.Empty;

        [Required, DisplayName, Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;
    }

    public IActionResult OnGet(string? provider, string? returnUrl)
    {
        logger.LogTrace("OnGet provider={provider} returnUrl={returnUrl}", provider, returnUrl);

        if (string.IsNullOrEmpty(provider))
        {
            logger.LogDebug("OnGet No provider was supplied.");
            return RedirectToPage("./Login", new { returnUrl });
        }

        var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        if (string.IsNullOrEmpty(remoteError))
        {
            ErrorMessage = $"Error from external provider: {remoteError}";
            return RedirectToPage("./Login", new { ReturnUrl });
        }

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            ErrorMessage = "Error loading external login information.";
            return RedirectToPage("./Login", new { ReturnUrl });
        }

        var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            logger.LogInformation("User {username} logged in with {loginProvider} provider.", info.Principal.Identity?.Name, info.LoginProvider);
            return LocalRedirect(ReturnUrl);
        }

        if (result.IsLockedOut)
        {
            return RedirectToPage("./Lockout");
        }

        ProviderDisplayName = info.ProviderDisplayName!;
        Input = new InputModel()
        {
            Username = info.Principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            DisplayName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty
        };
        return Page();
    }

    public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
    {
        logger.LogTrace("OnPostConfirmationAsync; returnUrl = {returnUrl} model={model}", returnUrl ?? "null", Input.ToJsonString());
        ReturnUrl = returnUrl ?? Url.Content("~/");

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            ErrorMessage = "Error loading external login information during confirmation.";
            return RedirectToPage("./Login", new { ReturnUrl });
        }

        ProviderDisplayName = info.ProviderDisplayName!;
        if (!ModelState.IsValid)
        {
            return Page();
        }

        ApplicationUser user = new()
        {
            DisplayName = Input.DisplayName,
            Email = Input.Username,
            EmailConfirmed = false,
            UserName = Input.Username
        };
        var result = await userManager.CreateAsync(user);
        if (result.Succeeded)
        {
            result = await userManager.AddLoginAsync(user, info);
            if (result.Succeeded)
            {
                string confirmationLink = await CreateConfirmationLinkAsync(user);
                await emailSender.SendConfirmationLinkAsync(user, user.Email, confirmationLink);
                if (userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = user.Email });
                }
                else
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(ReturnUrl);
                }
            }
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }

    /// <summary>
    /// Creates a confirmation link for the user.
    /// </summary>
    /// <param name="user">The user record.</param>
    /// <returns>The confirmation link.</returns>
    internal async Task<string> CreateConfirmationLinkAsync(ApplicationUser user)
    {
        var userId = await userManager.GetUserIdAsync(user);
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        return Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { userId, code }, protocol: Request.Scheme)
            ?? throw new ApplicationException("Cannot create URL for Account Confirmation");
    }
}
