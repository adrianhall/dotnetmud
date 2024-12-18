using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotnetmud.Web.Pages.Account;

[AllowAnonymous]
public class RegisterConfirmationModel : PageModel
{
    public void OnGet()
    {
    }
}
