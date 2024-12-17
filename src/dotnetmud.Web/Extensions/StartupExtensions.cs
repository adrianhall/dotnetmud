using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;

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
}
