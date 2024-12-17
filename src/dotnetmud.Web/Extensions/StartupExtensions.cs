using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;
using Microsoft.Extensions.Options;

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// A set of extensions to work with the Program.cs file for setting up
/// the application.
/// </summary>
public static class DotNetMud_StartupExtensions
{
    /// <summary>
    /// Adds the V8 JavaScript engine to the services collection.
    /// </summary>
    /// <param name="services">The services collection to modify.</param>
    /// <returns>The modified services collection.</returns>
    public static IServiceCollection AddJsEngine(this IServiceCollection services)
    {
        services.AddJsEngineSwitcher(options =>
        {
            options.AllowCurrentProperty = false;
            options.DefaultEngineName = V8JsEngine.EngineName;
        }).AddV8();
        return services;
    }

    /// <summary>
    /// Binds the provided configuration to the specified options type, then injects the options value into the services.
    /// </summary>
    /// <typeparam name="TOptions">The model type for the options.</typeparam>
    /// <param name="services">The services collection to modify.</param>
    /// <param name="configuration">The configuration element holding the options overrides.</param>
    /// <param name="defaultValue">The initial / default value of the options (can be null to generate a new one)</param>
    /// <returns>The resulting value for the options.</returns>
    public static TOptions BindOptions<TOptions>(this IServiceCollection services, IConfiguration configuration, TOptions? defaultValue = null)
        where TOptions : class, new()
    {
        defaultValue ??= Activator.CreateInstance<TOptions>();
        configuration.Bind(defaultValue);
        services.AddSingleton(Options.Create(defaultValue));
        return defaultValue;
    }
}
