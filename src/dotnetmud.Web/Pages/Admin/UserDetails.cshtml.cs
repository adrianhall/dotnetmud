using dotnetmud.Web.DataAnnotations;
using dotnetmud.Web.Database.Models;
using dotnetmud.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace dotnetmud.Web.Pages.Admin;

[Authorize(Roles = AppRoles.Administrator)]
public class UserDetailsModel(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<UserDetailsModel> logger
    ) : PageModel
{
    /// <summary>
    /// The user to display
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; } = new();

    /// <summary>
    /// True if this user is the current user.
    /// </summary>
    public bool IsCurrentUser { get; set; } = false;

    /// <summary>
    /// The list of available roles in the system.
    /// </summary>
    public List<string> AvailableRoles { get; set; } = [];

    /// <summary>
    /// The list of selected roles for this user.
    /// </summary>
    [BindProperty]
    public List<string> SelectedRoles { get; set; } = [];

    [TempData]
    public string? StatusMessage { get; set; }

    /// <summary>
    /// The input model definition for the user record.
    /// </summary>
    public class InputModel
    {
        [Required, EmailAddress, Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required, DisplayName, Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;

        [Display(Name = "Email Confirmed?")]
        public bool EmailConfirmed { get; set; } = false;

        [Display(Name = "Date account was created")]
        public string CreatedDate { get; set; } = string.Empty;

        [Display(Name = "Last login date")]
        public string LastLogin { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = [];

        public bool IsLockedOut { get; set; } = false;

        public string LockoutEnd { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        logger.LogTrace("OnGetAsync id={id}", id ?? "null");
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        Input = new()
        {
            DisplayName = user.DisplayName,
            Email = user.Email!,
            EmailConfirmed = user.EmailConfirmed,
            CreatedDate = user.CreatedAt.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss K"),
            LastLogin = user.LastLogin?.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss K") ?? "never", 
            Roles = (await userManager.GetRolesAsync(user))?.ToList() ?? [],
            IsLockedOut = user.LockoutEnd.HasValue,
            LockoutEnd = user.LockoutEnd.HasValue ? (user.LockoutEnd!.Value - DateTime.UtcNow).ToString("[-][d:]hh:mm:ss") : ""
        };
        IsCurrentUser = user.Id == userManager.GetUserId(User);
        AvailableRoles = await roleManager.Roles.OrderBy(r => r.Name).Select(r => r.Name!).ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostUnlockAccountAsync(string id)
    {
        logger.LogTrace("OnPostUnlockAccountAsync id={id}", id ?? "null");
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var result = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
        StatusMessage = result.Succeeded
            ? "Account unlocked."
            : $"Error unlocking account: {string.Join("<br/>\n", result.Errors.Select(x => x.Description))}";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteAccountAsync(string id)
    {
        logger.LogTrace("OnPostUnlockAccountAsync id={id}", id ?? "null");
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        if (user.Id == userManager.GetUserId(User))
        {
            StatusMessage = "You cannot delete your own account.";
            return RedirectToPage(new { id });
        }

        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            StatusMessage = "Account deleted.";
            return RedirectToPage("/Admin/Users");
        }
        else
        {
            StatusMessage = $"Error deleting account: {string.Join("<br/>\n", result.Errors.Select(x => x.Description))}";
            return RedirectToPage(new { id });
        }
    }
}
