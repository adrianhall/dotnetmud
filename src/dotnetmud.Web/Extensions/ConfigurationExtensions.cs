#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace Microsoft.Extensions.Configuration;

/// <summary>
/// A set of extension methods for the <see cref="IConfiguration"/> and associated classes.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Retrieves a connection string from the configuration, throwing an exception if it is not found.
    /// </summary>
    /// <param name="configuration">The configuration to search.</param>
    /// <param name="connectionStringName">The name of the connection string.</param>
    /// <returns>The value of the connection string.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the connection string is not found.</exception>
    public static string GetRequiredConnectionString(this IConfigurationRoot configuration, string connectionStringName)
    {
        string? connectionString = configuration.GetConnectionString(connectionStringName);
        return !string.IsNullOrWhiteSpace(connectionString) ? connectionString
            : throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");
    }
}
