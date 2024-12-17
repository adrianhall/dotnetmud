using Microsoft.AspNetCore.Identity;

namespace dotnetmud.Web.Models;

/// <summary>
/// A set of options for configuring the identity service.
/// </summary>
public class ServiceIdentityOptions
{
    /// <summary>
    /// The default administrator email address.
    /// </summary>
    public string DefaultAdministratorEmail { get; set; } = "admin@aspire.mud";

    /// <summary>
    /// The default administrator password - if not set, a password will be generated.
    /// </summary>
    public string? DefaultAdministratorPassword { get; set; }

    /// <summary>
    /// The lockout options that can be set.
    /// </summary>
    public LockoutOptions Lockout { get; set; } = new LockoutOptions();

    /// <summary>
    /// The password options that can be set.
    /// </summary>
    public PasswordOptions Password { get; set; } = new PasswordOptions();
}
