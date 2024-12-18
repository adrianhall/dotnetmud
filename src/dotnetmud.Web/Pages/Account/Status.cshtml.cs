using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotnetmud.Web.Pages.Account;

public class StatusModel : PageModel
{
    public string StatusMessage { get; set; } = string.Empty;

    public bool DisplayIndexLink { get; set; } = false;


    public void OnGet(string message = "", bool displayIndexLink = false)
    {
        StatusMessage = message;
        DisplayIndexLink = displayIndexLink;
    }
}
