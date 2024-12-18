namespace dotnetmud.Web.Services;

/// <summary>
/// Describes a service that renders a Razor View to a string.
/// </summary>
public interface IViewRenderingService
{
    /// <summary>
    /// Renders an email template without a model.
    /// </summary>
    /// <param name="viewName">The name of the view.</param>
    /// <returns>The rendered HTML email template.</returns>
    Task<string> RenderViewAsync(string viewName);

    /// <summary>
    /// Renders an email template with a model.
    /// </summary>
    /// <typeparam name="TModel">The type of the view model.</typeparam>
    /// <param name="viewName">The name of the view.</param>
    /// <param name="model">The view model.</param>
    /// <returns>The rendered HTML email template.</returns>
    Task<string> RenderViewAsync<TModel>(string viewName, TModel model);
}
