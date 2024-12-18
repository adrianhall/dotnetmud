using dotnetmud.Web.DataAnnotations;
using dotnetmud.Web.Database.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace dotnetmud.Web.Pages.Account;

public class RegisterModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IEmailSender<ApplicationUser> emailSender,
    ILogger<RegisterModel> logger
    ) : PageModel
{
    /// <summary>
    /// The form model for the registration page.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; } = new();

    /// <summary>
    /// The returnUrl parameter from the query string.
    /// </summary>
    public string ReturnUrl { get; set; } = string.Empty;

    /// <summary>
    /// A temporary error message to display.
    /// </summary>
    [TempData]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// The input model for the registration page.
    /// </summary>
    public class InputModel
    {
        [Required, EmailAddress, Display(Name = "Email Address")]
        public string Username { get; set; } = string.Empty;

        [Required, DisplayName, Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        logger.LogTrace("OnGetAsync; returnUrl = {returnUrl}", returnUrl ?? "null");
        await SetRequiredPropertiesAsync(returnUrl);

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        logger.LogTrace("OnPostAsync; returnUrl = {returnUrl}", returnUrl ?? "null");
        await SetRequiredPropertiesAsync(returnUrl);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new ApplicationUser()
        {
            DisplayName = Input.DisplayName,
            Email = Input.Username,
            EmailConfirmed = false,
            UserName = Input.Username
        };
        var result = await userManager.CreateAsync(user, Input.Password);
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

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }

    /// <summary>
    /// Sets the required properties for the page.
    /// </summary>
    /// <remarks>
    /// This is async in case we want to include anything that interacts with the database.
    /// </remarks>
    /// <param name="returnUrl">The provided return URL</param>
    internal Task SetRequiredPropertiesAsync(string? returnUrl)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        return Task.CompletedTask;
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
        return Url.Page("/Account/ConfirmEmail", pageHandler: null,  values: new { userId, code }, protocol: Request.Scheme)
            ?? throw new ApplicationException("Cannot create URL for Account Confirmation");
    }
}
