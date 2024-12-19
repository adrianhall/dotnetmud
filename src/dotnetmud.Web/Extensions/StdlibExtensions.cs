using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace dotnetmud.Web;

public static class StdlibExtensions
{
    private static readonly Lazy<JsonSerializerOptions> SerializerOptions = new(GetJsonSerializerOptions);

    /// <summary>
    /// Converts a dictionary (generally environment variables) to a debug HTML view.
    /// </summary>
    /// <param name="dict">The dictionary to convert.</param>
    /// <param name="className">The name of the class to append to the table.</param>
    /// <param name="envExclusions">If true, exclude problematic environment variables</param>
    /// <returns></returns>
    public static string GetDebugHtmlView(this IDictionary dict, string className = "aspnetcore-debugview", bool envExclusions = false)
    {
        List<string> listOfKeys = dict.Keys.Cast<string>().ToList();
        listOfKeys.Sort(StringComparer.Ordinal);
        List<string> excludedValues = !envExclusions ? [] : [
            "ASPNETCORE_AUTO_RELOAD_WS_KEY",
            "DOTNET_STARTUP_HOOKS",
            "Path",
            "PSModulePath"
        ];

        StringBuilder builder = new();
        builder.AppendLine($"<table class=\"{className}\">");
        builder.AppendLine("<tr><th>Key</th><th>Value</th></tr>");
        foreach (var key in listOfKeys)
        {
            if (!excludedValues.Contains(key, StringComparer.OrdinalIgnoreCase))
            {
                builder.AppendLine($"<tr><td>{key}</td><td>{dict[key]}</td></tr>");
            }
        }
        builder.AppendLine("</table>");
        return builder.ToString();
    }

    /// <summary>
    /// Converts a given object to a JSON string.
    /// </summary>
    public static string ToJsonString(this object? obj)
        => obj is null ? "null" : JsonSerializer.Serialize(obj, SerializerOptions.Value);

    /// <summary>
    /// Returns the JSON serializer options for logging purposes.
    /// </summary>
    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        };
    }
}
