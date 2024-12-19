using dotnetmud.DataTables.Interfaces;

namespace dotnetmud.DataTables.Models;

/// <summary>
/// Represents search/filter definition and value.
/// </summary>
public class DataTablesSearch(string value = "", bool isRegex = false) : IDataTablesSearch
{
    /// <inheritdoc />
    public string Value { get; } = value;
    /// <inheritdoc />
    public bool IsRegex { get; } = isRegex;
}
