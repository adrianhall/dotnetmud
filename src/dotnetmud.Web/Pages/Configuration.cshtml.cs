using Microsoft.AspNetCore.Mvc.RazorPages;

namespace dotnetmud.Web.Pages;

public class ConfigurationModel(
    IWebHostEnvironment env,
    ILogger<ConfigurationModel> logger
    ) : PageModel
{
    public void OnGet()
    {
        if (!env.IsDevelopment())
        {
            logger.LogWarning("Configuration Page accessed by {ipaddr} in production mode", HttpContext.Connection.RemoteIpAddress);
            Response.Redirect("/Index");
        }
    }
}
