using dotnetmud.DataTables.Interfaces;

namespace dotnetmud.DataTables.Models;

/// <summary>
/// Represents a column in a DataTables request.
/// </summary>
public class DataTablesColumn(
    string name,
    string field,
    bool searchable,
    bool sortable,
    IDataTablesSearch search) : IDataTablesColumn
{
    /// <inheritdoc />
    public string Field { get; } = field;

    /// <inheritdoc />
    public string Name { get; } = name;

    /// <inheritdoc />
    public IDataTablesSearch Search { get; } = searchable ? search ?? new DataTablesSearch() : null;

    /// <inheritdoc />
    public bool IsSearchable { get; } = searchable;

    /// <inheritdoc />
    public IDataTablesSort Sort { get; private set; }

    /// <inheritdoc />
    public bool IsSortable { get; } = sortable;

    /// <inheritdoc />
    public bool SetSort(int order, string direction)
    {
        if (!IsSortable)
        {
            return false;
        }

        Sort = new DataTablesSort(order, direction);
        return true;
    }
}
{
}
