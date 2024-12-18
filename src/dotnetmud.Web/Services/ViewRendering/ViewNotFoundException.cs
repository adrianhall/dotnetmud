namespace dotnetmud.Web.Services;

/// <summary>
/// An exception that is thrown when a view cannot be found.
/// </summary>
public class ViewNotFoundException : Exception
{
    public ViewNotFoundException()
    {
    }

    public ViewNotFoundException(string? message) : base(message)
    {
    }

    public ViewNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public required IList<string> SearchedLocations { get; init; }
}
