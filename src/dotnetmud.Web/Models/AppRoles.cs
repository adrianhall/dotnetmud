namespace dotnetmud.Web.Models;

/// <summary>
/// The list of roles that are available in the application.
/// </summary>
public static class AppRoles
{
    /// <summary>
    /// The administrator - can do anything.
    /// </summary>
    public const string Administrator = "administrator";

    /// <summary>
    /// The list of all roles (for creation).
    /// </summary>
    public static readonly IEnumerable<string> AllRoles = [Administrator ];
}
