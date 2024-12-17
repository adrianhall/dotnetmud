﻿#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace Microsoft.AspNetCore.Identity;

/// <summary>
/// A set of extension methods for the ASP.NET Core Identity service.
/// </summary>
public static class IdentityExtensions
{
    /// <summary>
    /// Copies the values from one <see cref="LockoutOptions"/> instance to another.
    /// </summary>
    /// <param name="options">The original options.</param>
    /// <param name="other">The options to copy into the original options.</param>
    public static void CopyFrom(this LockoutOptions options, LockoutOptions other)
    {
        options.AllowedForNewUsers = other.AllowedForNewUsers;
        options.DefaultLockoutTimeSpan = other.DefaultLockoutTimeSpan;
        options.MaxFailedAccessAttempts = other.MaxFailedAccessAttempts;
    }

    /// <summary>
    /// Copies the values from one <see cref="PasswordOptions"/> instance to another.
    /// </summary>
    /// <param name="options">The original options.</param>
    /// <param name="other">The options to copy into the original options.</param>
    public static void CopyFrom(this PasswordOptions options, PasswordOptions other)
    {
        options.RequireDigit = other.RequireDigit;
        options.RequiredLength = other.RequiredLength;
        options.RequiredUniqueChars = other.RequiredUniqueChars;
        options.RequireLowercase = other.RequireLowercase;
        options.RequireNonAlphanumeric = other.RequireNonAlphanumeric;
        options.RequireUppercase = other.RequireUppercase;
    }
}
