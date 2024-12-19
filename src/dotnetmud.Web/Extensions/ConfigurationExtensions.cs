#pragma warning disable IDE0130 // Namespace does not match folder structure

using System.Text;

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

    /// <summary>
    /// A better GetDebugView() method that excludes the values of environment variables optionally.
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    public static string GetDebugHtmlView(this IConfigurationRoot root, bool includeEnvironment = true)
    {
        void RecurseChildren(StringBuilder stringBuilder, IEnumerable<IConfigurationSection> children, string indent)
        {
            stringBuilder.AppendLine("<ul class=\"aspnetcore-configuration\">");
            foreach (IConfigurationSection child in children)
            {
                (string? value, IConfigurationProvider? provider) = GetValueAndProvider(root, child.Path);

                if (!includeEnvironment && (provider?.ToString()?.StartsWith("Environment") ?? false))
                {
                    continue;
                }

                if (provider is not null)
                {
                    stringBuilder.AppendLine($"<li>{indent}<span class=\"key\">{child.Key}</span>=<span class=\"value\">{value}</span> <span class=\"provider\">({provider})</span></li>");

                }
                else if (!string.IsNullOrEmpty(child.Key))
                {
                    string childvalue = string.IsNullOrEmpty(child.Value) ? "" : $"=<span class=\"value\">{child.Value}</span>";
                    stringBuilder.AppendLine($"<li>{indent}<span class=\"key\">{child.Key}</span>{childvalue}</li>");
                }

                RecurseChildren(stringBuilder, child.GetChildren(), indent + "  ");
            }
            stringBuilder.AppendLine("</ul>");
        }

        var builder = new StringBuilder();
        RecurseChildren(builder, root.GetChildren(), "");
        return builder.ToString();
    }

    private static (string? Value, IConfigurationProvider? Provider) GetValueAndProvider(IConfigurationRoot root, string key)
    {
        foreach (IConfigurationProvider provider in root.Providers.Reverse())
        {
            if (provider.TryGet(key, out string? value))
            {
                return (value, provider);
            }
        }

        return (null, null);
    }
}
