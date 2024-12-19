using dotnetmud.Web.Database.Models;
using dotnetmud.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace dotnetmud.Web.Pages.Admin;

[Authorize(Roles = AppRoles.Administrator)]
public class UsersModel(
    UserManager<ApplicationUser> userManager,
    ILogger<UsersModel> logger
    ) : PageModel
{
    public void OnGet()
    {
    }

}
