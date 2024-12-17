using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace dotnetmud.Web.Database.Models;

public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(50)]
    public required string DisplayName { get; set; }
}
