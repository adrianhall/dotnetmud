using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace dotnetmud.Web;

public static class StdlibExtensions
{
    private static readonly Lazy<JsonSerializerOptions> SerializerOptions = new(GetJsonSerializerOptions);

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
