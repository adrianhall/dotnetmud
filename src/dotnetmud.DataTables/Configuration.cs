using dotnetmud.DataTables.Interfaces;
using dotnetmud.DataTables.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace dotnetmud.DataTables;

/// <summary>
/// Handles DataTables.AspNet registration and holds default (global) configuration options.
/// </summary>
public static class Configuration
{
    /// <summary>
    /// Get's DataTables.AspNet runtime options for server-side processing.
    /// </summary>
    public static IDataTablesOptions Options { get; private set; } = new DataTablesOptions();

    /// <summary>
    /// Provides DataTables.AspNet registration for AspNet5 projects.
    /// </summary>
    /// <param name="services">Service collection for dependency injection.</param>
    public static void AddDataTables(this IServiceCollection services) 
    {
        services.AddDataTables(new DataTablesOptions()); 
    }

    /// <summary>
    /// Provides DataTables.AspNet registration for AspNet5 projects.
    /// </summary>
    /// <param name="services">Service collection for dependency injection.</param>
    /// <param name="options">DataTables.AspNet options.</param>
    public static void AddDataTables(this IServiceCollection services, IDataTablesOptions options) 
    { 
        services.AddDataTables(options, new DataTablesModelBinder()); 
    }

    /// <summary>
    /// Provides DataTables.AspNet registration for AspNet5 projects.
    /// </summary>
    /// <param name="services">Service collection for dependency injection.</param>
    /// <param name="requestModelBinder">Request model binder to use when resolving 'IDataTablesRequest' models.</param>
    public static void AddDataTables(this IServiceCollection services, DataTablesModelBinder requestModelBinder) 
    { 
        services.AddDataTables(new DataTablesOptions(), requestModelBinder); 
    }

    /// <summary>
    /// Provides DataTables.AspNet registration for AspNet5 projects.
    /// </summary>
    /// <param name="services">Service collection for dependency injection.</param>
    /// <param name="parseRequestAdditionalParameters">Function to evaluante and parse aditional parameters sent within the request (user-defined parameters).</param>
    /// <param name="parseResponseAdditionalParameters">Indicates whether response aditional parameters parsing is enabled or not.</param>
    public static void AddDataTables(this IServiceCollection services, Func<ModelBindingContext, IDictionary<string, object>> parseRequestAdditionalParameters, bool parseResponseAdditionalParameters) 
    { 
        services.AddDataTables(new DataTablesOptions(), new DataTablesModelBinder(), parseRequestAdditionalParameters, parseResponseAdditionalParameters); 
    }

    /// <summary>
    /// Provides DataTables.AspNet registration for AspNet5 projects.
    /// </summary>
    /// <param name="options">DataTables.AspNet options.</param>
    /// <param name="requestModelBinder">Model binder to use when resolving 'IDataTablesRequest' model.</param>
    public static void AddDataTables(this IServiceCollection services, IDataTablesOptions options, DataTablesModelBinder requestModelBinder) 
    { 
        services.AddDataTables(options, requestModelBinder, null, false);
    }

    /// <summary>
    /// Provides DataTables.AspNet registration for ASP.NET Core projects.
    /// </summary>
    /// <param name="services">Service collection for dependency injection.</param>
    /// <param name="options">DataTables.AspNet options.</param>
    /// <param name="requestModelBinder">Request model binder to use when resolving 'IDataTablesRequest' models.</param>
    /// <param name="parseRequestAdditionalParameters">Function to evaluate and parse aditional parameters sent within the request (user-defined parameters).</param>
    /// <param name="enableResponseAdditionalParameters">Indicates whether response aditional parameters parsing is enabled or not.</param>
    public static void AddDataTables(this IServiceCollection services, IDataTablesOptions options, DataTablesModelBinder requestModelBinder, Func<ModelBindingContext, IDictionary<string, object>> parseRequestAdditionalParameters, bool enableResponseAdditionalParameters)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(requestModelBinder);
        Options = options;

        if (parseRequestAdditionalParameters != null)
        {
            Options.EnableRequestAdditionalParameters();
            requestModelBinder.ParseAdditionalParameters = parseRequestAdditionalParameters;
        }

        if (enableResponseAdditionalParameters)
        {
            Options.EnableResponseAdditionalParameters();
        }

        // Should be inserted into first position because there is a generic
        // binder which could end up resolving/binding model incorrectly.
        services.Configure<MvcOptions>(c => c.ModelBinderProviders.Insert(0, new ModelBinderProvider(requestModelBinder)));
    }


    internal class ModelBinderProvider(IModelBinder modelBinder = null) : IModelBinderProvider
    {
        public IModelBinder ModelBinder { get; private set; } = modelBinder;

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (IsBindable(context.Metadata.ModelType))
            {
                ModelBinder ??= new DataTablesModelBinder();
                return ModelBinder;
            }
            else
            {
                return null;
            }
        }

        private static bool IsBindable(Type type) 
            => type.Equals(typeof(IDataTablesRequest));
    }
}
