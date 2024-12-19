namespace dotnetmud.DataTables.Interfaces;

/// <summary>
/// Defines the DataTables column members.
/// </summary>
public interface IDataTablesColumn
{
    /// <summary>
    /// Column name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Column field.
    /// </summary>
    string Field { get; }

    /// <summary>
    /// Column searchable indicator.
    /// </summary>
    bool IsSearchable { get; }

    /// <summary>
    /// Column search definition; null if column is not searchable.
    /// </summary>
    IDataTablesSearch Search { get; }

    /// <summary>
    /// Column sortable indicator.
    /// </summary>
    bool IsSortable { get; }

    /// <summary>
    /// Sort definition; null if column is not sortable.
    /// </summary>
    IDataTablesSort Sort { get; }

    /// <summary>
    /// Sets the sort definition for the columns.
    /// </summary>
    /// <param name="order">The sort order.</param>
    /// <param name="direction">The sort direction.</param>
    /// <returns>True if sort could be set; false otherwise.</returns>
    bool SetSort(int order, string direction);
}
