using dotnetmud.DataTables.Interfaces;

namespace dotnetmud.DataTables.Models;

/// <summary>
/// Represents sort/ordering for columns.
/// </summary>
public class DataTablesSort(int order, string direction) : IDataTablesSort
{
    /// <inheritdoc />
    public int Order { get; } = order;
    /// <inheritdoc />
    public SortDirection Direction { get; } = ParseSortDirection(direction);

    private static SortDirection ParseSortDirection(string direction) 
        => (direction ?? "").ToLowerInvariant().Equals(Configuration.Options.RequestNameConvention.SortDescending)
        ? SortDirection.Descending // Descending sort should be explicitly set.
        : SortDirection.Ascending; // Default (when set or not) is ascending sort.
}
