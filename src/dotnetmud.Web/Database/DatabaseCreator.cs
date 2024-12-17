using dotnetmud.Web.Database.Models;
using dotnetmud.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace dotnetmud.Web.Database;

/// <summary>
/// Initializes the database for the application.  This is generally fulfilled
/// by the dependency injection container and initiated during program startup.
/// </summary>
/// <remarks>
/// There are two values you can set within the appsettings:
/// 
/// * <c>Database:ResetDatabaseOnStartup</c> will delete the database on startup.
/// * <c>Database:MigrateDatabaseOnStartup</c> will migrate the database on startup, then return (without seeding).
/// 
/// In the absence of the MigrateDatabaseOnStartup setting, the database will be
/// seeded with a default administrator user and the initial roles.
/// </remarks>
public class DatabaseCreator(
    AppDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration,
    IOptions<ServiceIdentityOptions> identityOptions,
    ILogger<DatabaseCreator> logger
    ) : IDatabaseCreator
{
    /// <summary>
    /// This is used by the password generator.
    /// </summary>
    private static readonly string[] ValidPasswordCharacters = [
        "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
        "abcdefghijkmnopqrstuvwxyz",    // lowercase
        "0123456789",                   // digits
        "!@$?_-"                        // non-alphanumeric
    ];

    public async Task InitializeDatabaseAsync(CancellationToken cancellationToken = default)
    {
        logger.LogTrace("InitializeDatabaseAsync called.");

        DatabaseOptions databaseOptions = new();
        configuration.GetSection("Database").Bind(databaseOptions);
        logger.LogDebug("Database options: {DatabaseOptions}", databaseOptions.ToJsonString());
        logger.LogDebug("Connection string: {ConnectionString}", context.Database.GetConnectionString());

        if (databaseOptions.ResetDatabaseOnStartup)
        {
            logger.LogDebug("Attempting to delete database");
            bool isDeleted = await context.Database.EnsureDeletedAsync(cancellationToken);
            if (isDeleted)
            {
                logger.LogInformation("Database deleted successfully.");
            }
            else
            {
                logger.LogWarning("Requested database deletion failed (the database may not exist).");
            }
        }

        if (databaseOptions.MigrateDatabaseOnStartup)
        {
            logger.LogDebug("Attempting database migration.");
            await context.Database.MigrateAsync(cancellationToken);
            logger.LogDebug("Database migration completed.");
            return;
        }

        logger.LogDebug("Attempting to create the database.");
        bool isCreated = await context.Database.EnsureCreatedAsync(cancellationToken);
        if (isCreated)
        {
            logger.LogInformation("Database created successfully.");
        }
        else
        {
            logger.LogWarning("Requested database creation failed (the database may already exist).");
        }

        // Ensure all the roles are created.
        foreach (string roleName in AppRoles.AllRoles)
        {
            bool roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                logger.LogDebug("Role {roleName} does not exist - attempting to create.", roleName);
                IdentityResult createRoleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (createRoleResult.Succeeded)
                {
                    logger.LogInformation("Role {roleName} created.", roleName);
                }
                else
                {
                    logger.LogError("Role {roleName} creation failed: {Errors}", roleName, createRoleResult.Errors.ToJsonString());
                    throw new ApplicationException($"Cannot create role {roleName}");
                }
            }
        }

        // If there is no users, create one.
        bool hasUsers = await userManager.Users.AnyAsync(cancellationToken: cancellationToken);
        if (!hasUsers)
        {
            logger.LogDebug("No users exist - attempting to create a default administrator user.");

            ApplicationUser user = new()
            {
                DisplayName = "Default Administrator",
                Email = identityOptions.Value.DefaultAdministratorEmail,
                EmailConfirmed = true,
                UserName = identityOptions.Value.DefaultAdministratorEmail
            };

            string password = identityOptions.Value.DefaultAdministratorPassword ?? GenerateRandomPassword();
            IdentityResult createUserResult = await userManager.CreateAsync(user, password);
            if (!createUserResult.Succeeded)
            {
                logger.LogError("Cannot create default administrator user: {Errors}", createUserResult.Errors.ToJsonString());
                throw new ApplicationException("Cannot create default administrator user.");
            }

            IdentityResult addRolesResult = await userManager.AddToRoleAsync(user, AppRoles.Administrator);
            if (!addRolesResult.Succeeded)
            {
                logger.LogError("Cannot add default user to Administrator role: {Errors}", addRolesResult.Errors.ToJsonString());
                throw new ApplicationException("Cannot add default user to Administrator role.");
            }

            logger.LogInformation("Created default admin user.  email: {Email}, password: {Password}", user.Email, password);
        }
    }

    /// <summary>
    /// Generates a Random Password respecting the given strength requirements.
    /// </summary>
    /// <param name="opts">A valid PasswordOptions object containing the password strength requirements.</param>
    /// <returns>A random password</returns>
    internal string GenerateRandomPassword()
    {
        PasswordOptions opts = identityOptions.Value.Password;


        Random random = new();
        List<char> chars = [];

        if (opts.RequireUppercase)
        {
            chars.Insert(random.Next(0, chars.Count), ValidPasswordCharacters[0][random.Next(0, ValidPasswordCharacters[0].Length)]);
        }

        if (opts.RequireLowercase)
        {
            chars.Insert(random.Next(0, chars.Count), ValidPasswordCharacters[1][random.Next(0, ValidPasswordCharacters[1].Length)]);
        }

        if (opts.RequireDigit)
        {
            chars.Insert(random.Next(0, chars.Count), ValidPasswordCharacters[2][random.Next(0, ValidPasswordCharacters[2].Length)]);
        }

        if (opts.RequireNonAlphanumeric)
        {
            chars.Insert(random.Next(0, chars.Count), ValidPasswordCharacters[3][random.Next(0, ValidPasswordCharacters[3].Length)]);
        }

        for (int i = chars.Count; i < opts.RequiredLength || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
        {
            string rcs = ValidPasswordCharacters[random.Next(0, ValidPasswordCharacters.Length)];
            chars.Insert(random.Next(0, chars.Count), rcs[random.Next(0, rcs.Length)]);
        }

        return new string([.. chars]);
    }

    /// <summary>
    /// The options for the database initialization.
    /// </summary>
    class DatabaseOptions
    {
        /// <summary>
        /// If true, the database will be reset on startup.
        /// </summary>
        public bool ResetDatabaseOnStartup { get; set; }

        /// <summary>
        /// If true, the database will be migrated on startup (without seeding).
        /// </summary>
        public bool MigrateDatabaseOnStartup { get; set; }
    }
}
