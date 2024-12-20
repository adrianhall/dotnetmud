using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
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
    [BindProperty]
    public IDataTablesRequest? DataTablesRequest { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    /// <summary>
    /// The field mapping for the datatables.net API.
    /// </summary>
    private readonly Dictionary<string, string> fieldMapping = new()
    {
        ["id"] = "Id",
        ["emailAddress"] = "Email",
        ["emailConfirmed"] = "EmailConfirmed",
        ["displayName"] = "DisplayName",
        ["roles"] = "Roles"
    };

    /// <summary>
    /// Displays the users table HTML.
    /// </summary>
    public void OnGet()
    {
    }

    /// <summary>
    /// Handler for the datatables.net API AJAX request.
    /// </summary>
    /// <returns>A datatables.net result response.</returns>
    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogTrace("OnPostAsync model={model}", DataTablesRequest.ToJsonString());

        if (DataTablesRequest is null)
        {
            return BadRequest("Invalid request");
        }

        IQueryable<ApplicationUser> queryable = userManager.Users;
        IQueryable<ApplicationUser> filteredUsers = queryable.GlobalFilterBy(DataTablesRequest.Search, DataTablesRequest.Columns);
        List<ApplicationUser> pagedUsers = await filteredUsers
            .SortBy(DataTablesRequest.Columns ?? [], fieldMapping)
            .GetPage(DataTablesRequest)
            .ToListAsync();
        int totalCount = await queryable.CountAsync();
        int filteredCount = queryable == filteredUsers ? totalCount : await filteredUsers.CountAsync();

        List<ResultModel> pagedResults = [];
        foreach (var user in pagedUsers)
        {
            pagedResults.Add(new ResultModel(user) { Roles = string.Join(", ", await userManager.GetRolesAsync(user)) });
        }
        DataTablesResponse response = DataTablesResponse.Create(DataTablesRequest, totalCount, filteredCount, pagedResults);
        return new DataTablesJsonResult(response);
    }

    /// <summary>
    /// The model class for the results that the datatables.net reader requires.
    /// </summary>
    /// <param name="user"></param>
    public class ResultModel(ApplicationUser user)
    {
        public string Id { get; set; } = user.Id;
        public string EmailAddress { get; set; } = user.Email!;
        public bool EmailConfirmed { get; set; } = user.EmailConfirmed;
        public string DisplayName { get; set; } = user.DisplayName;
        public string Roles { get; set; } = string.Empty;
    }

}
