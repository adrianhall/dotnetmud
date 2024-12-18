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

    /// <summary>
    /// Retrieves a section of the configuration and binds it to an instance of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the options.</typeparam>
    /// <param name="configuration">The configuration section to bind to.</param>
    /// <param name="defaultValue">the optional default value for the instance.</param>
    /// <returns>The bound instance.</returns>
    public static T BindWithDefaults<T>(this IConfiguration configuration, T? defaultValue = null) where T : class, new()
    {
        defaultValue ??= Activator.CreateInstance<T>();
        configuration.Bind(defaultValue);
        return defaultValue;
    }
}
