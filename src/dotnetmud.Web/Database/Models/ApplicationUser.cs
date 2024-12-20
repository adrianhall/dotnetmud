using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace dotnetmud.Web.Database.Models;

public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// The display name for the user.
    /// </summary>
    [Required, MaxLength(50)]
    public required string DisplayName { get; set; }

    /// <summary>
    /// The date/time that the user was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The date/time that the user last logged in.
    /// </summary>
    public DateTimeOffset? LastLogin { get; set; }
}
