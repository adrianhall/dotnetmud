﻿using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace dotnetmud.Web.Services;

public class ViewRenderingService(
    IRazorViewEngine viewEngine,
    ITempDataProvider tempDataProvider,
    IServiceProvider serviceProvider
    ) : IViewRenderingService
{
    /// <inheritdoc />
    public Task<string> RenderViewAsync(string viewName)
    {
        ViewDataDictionary viewData = new(new EmptyModelMetadataProvider(), new ModelStateDictionary());
        return RenderViewToStringAsync(viewName, viewData);
    }

    /// <inheritdoc />
    public Task<string> RenderViewAsync<TModel>(string viewName, TModel model)
    {
        ViewDataDictionary<TModel> viewData = new(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };
        return RenderViewToStringAsync(viewName, viewData);
    }

    /// <summary>
    /// Renders a view to a string using the provided view data dictionary.
    /// </summary>
    /// <param name="viewName">The name of the view.</param>
    /// <param name="viewData">The view data dictionary.</param>
    /// <returns>The rendered view (asynchronously)</returns>
    /// <exception creg="ViewNotFoundException">Thrown if the view cannot be found.</exception>
    internal async Task<string> RenderViewToStringAsync(string viewName, ViewDataDictionary viewData)
    {
        HttpContext httpContext = new DefaultHttpContext() { RequestServices = serviceProvider };
        ActionContext actionContext = new(httpContext, new RouteData(), new ActionDescriptor());
        IView view = FindView(actionContext, viewName);

        using var output = new StringWriter();
        TempDataDictionary tempData = new(actionContext.HttpContext, tempDataProvider);
        ViewContext viewContext = new(actionContext, view, viewData, tempData, output, new HtmlHelperOptions());
        await view.RenderAsync(viewContext);
        return output.ToString();
    }

    /// <summary>
    /// Finds the specified view within this library.
    /// </summary>
    /// <param name="context">The action context.</param>
    /// <param name="viewName">The view name.</param>
    /// <returns>The view entity.</returns>
    /// <exception cref="EmailTemplateNotFoundException">Thrown if the view cannot be found.</exception>
    internal IView FindView(ActionContext context, string viewName)
    {
        var getViewResult = viewEngine.GetView(null, viewName, isMainPage: true);
        if (getViewResult.Success)
        {
            return getViewResult.View;
        }

        var findViewResult = viewEngine.FindView(context, viewName, isMainPage: true);
        if (findViewResult.Success)
        {
            return findViewResult.View;
        }

        var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
        var errorMessage = string.Join(Environment.NewLine, [
            $"Unable to find view '{viewName}'.  The following locations were searched:",
            ..searchedLocations
        ]);
        throw new ViewNotFoundException(errorMessage) { SearchedLocations = searchedLocations.ToList() };
    }
}
