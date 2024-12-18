#pragma warning disable IDE0130 // Namespace does not match folder structure

using dotnetmud.Web.Models;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.AspNetCore.Identity;

/// <summary>
/// A set of extension methods for the ASP.NET Core Identity service.
/// </summary>
public static class IdentityExtensions
{
    /// <summary>
    /// Extension method to add the Facebook OAuth provider to the authentication builder if the configuration exists.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="configuration">The configuration for the provider.</param>
    /// <returns>The authentication builder, for chaining.</returns>
    public static AuthenticationBuilder AddFacebookOAuthProvider(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        OAuthProviderOptions options = configuration.BindWithDefaults<OAuthProviderOptions>();
        if (options.Enabled)
        {
            builder.AddFacebook(cfg =>
            {
                cfg.ClientId = options.ClientId;
                cfg.ClientSecret = options.ClientSecret;
            });
        }
        return builder;
    }

    /// <summary>
    /// Extension method to add the Google OAuth provider to the authentication builder if the configuration exists.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="configuration">The configuration for the provider.</param>
    /// <returns>The authentication builder, for chaining.</returns>
    public static AuthenticationBuilder AddGoogleOAuthProvider(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        OAuthProviderOptions options = configuration.BindWithDefaults<OAuthProviderOptions>();
        if (options.Enabled)
        {
            builder.AddGoogle(cfg =>
            {
                cfg.ClientId = options.ClientId;
                cfg.ClientSecret = options.ClientSecret;
            });
        }
        return builder;
    }

    /// <summary>
    /// Extension method to add the Microsoft Account OAuth provider to the authentication builder if the configuration exists.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="configuration">The configuration for the provider.</param>
    /// <returns>The authentication builder, for chaining.</returns>
    public static AuthenticationBuilder AddMicrosoftOAuthProvider(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        OAuthProviderOptions options = configuration.BindWithDefaults<OAuthProviderOptions>();
        if (options.Enabled)
        {
            builder.AddMicrosoftAccount(cfg =>
            {
                cfg.ClientId = options.ClientId;
                cfg.ClientSecret = options.ClientSecret;
            });
        }
        return builder;
    }

    /// <summary>
    /// Binds the identity options from the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The <see cref="ServiceIdentityOptions"/></returns>
    public static ServiceIdentityOptions BindIdentityOptions(this IConfiguration configuration, string sectionName)
    {
        ServiceIdentityOptions options = new();
        IConfiguration identityConfiguration = configuration.GetSection(sectionName);
        identityConfiguration.GetSection("Password").Bind(options.Password);
        identityConfiguration.GetSection("Lockout").Bind(options.Lockout);

        // Misc settings
        if (identityConfiguration[nameof(options.DefaultAdministratorEmail)] is string adminEmail)
        {
            options.DefaultAdministratorEmail = adminEmail;
        }

        if (identityConfiguration[nameof(options.DefaultAdministratorPassword)] is string adminPassword)
        {
            options.DefaultAdministratorPassword = adminPassword;
        }

        if (identityConfiguration[nameof(options.EnableLockout)] is string enableLockout)
        {
            options.EnableLockout = enableLockout.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        return options;
    }

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

    /// <summary>
    /// Finds a user by either their username or email address.
    /// </summary>
    /// <typeparam name="TUser">The type of the user record.</typeparam>
    /// <param name="userManager">The user manager for ASP.NET Identity</param>
    /// <param name="username">The username to find.</param>
    /// <returns>The user record, or null if not found.</returns>
    public static async Task<TUser?> FindUserAsync<TUser>(this UserManager<TUser> userManager, string username) where TUser : IdentityUser
        => await userManager.FindByNameAsync(username) ?? await userManager.FindByEmailAsync(username);
}
