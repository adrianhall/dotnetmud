namespace dotnetmud.Web.Database;

/// <summary>
/// A service definition for the database creator service.
/// </summary>
public interface IDatabaseCreator
{
    /// <summary>
    /// Initializes the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe.</param>
    Task InitializeDatabaseAsync(CancellationToken cancellationToken = default);
}


