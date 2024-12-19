namespace dotnetmud.Web.Models;

#nullable disable

/// <summary>
/// Definition of the request from the datatables.net service.
/// </summary>
/// <see cref="https://datatables.net/manual/server-side#Sent-parameters"/>
public class DataTablesRequest
{
    /// <summary>
    /// <para>Draw counter</para>
    /// <para>This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in 
    /// sequence by DataTables (Ajax requests are asynchronous and thus can return out of sequence). This is used as part of the 
    /// <c>draw</c> response parameter.</para>
    /// </summary>
    public int Draw { get; set; }

    /// <summary>
    /// An array defining all the columns being requested.
    /// </summary>
    public IEnumerable<ColumnRequest> Columns { get; set; }

    /// <summary>
    /// An array defining how many columns are being ordered upon.
    /// </summary>
    public IEnumerable<OrderRequest> Order { get; set; }

    /// <summary>
    /// Paging first record indicator. This is the start point in the current data set (0 index based - i.e. 0 is the first record).
    /// </summary>
    public int Start { get; set; }

    /// <summary>
    /// Number of records that the table can display in the current draw. It is expected that the number of records returned will be equal 
    /// to this number, unless the server has fewer records to return. Note that this can be -1 to indicate that all records should be 
    /// returned (although that negates any benefits of server-side processing!)
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Global search value. To be applied to all columns which have searchable as true.
    /// </summary>
    public SearchRequest Search { get; set; }

    /// <summary>
    /// The definition of a requested column.
    /// </summary>
    public class ColumnRequest
    {
        /// <summary>
        /// Column's data source.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Column's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If true, the client requests to search the column.
        /// </summary>
        public bool Searchable { get; set; }

        /// <summary>
        /// If true, the client requests to sort the column.
        /// </summary>
        public bool Orderable { get; set; }

        /// <summary>
        /// Search value to apply to this specific column.
        /// </summary>
        public SearchRequest Search { get; set; }
    }

    /// <summary>
    /// Definition of the ordering for a column.
    /// </summary>
    public class OrderRequest
    {
        /// <summary>
        /// Column to which ordering should be applied. This is an index reference to the columns array of information that is also submitted to the server.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Ordering direction for this column. It will be asc or desc to indicate ascending ordering or descending ordering, respectively.
        /// </summary>
        public string Dir { get; set; }
    }

    /// <summary>
    /// Definition of a requested search.
    /// </summary>
    public class SearchRequest
    {
        /// <summary>
        /// The value to search for.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Flag to indicate if the search term for this column should be treated as regular expression (true) or not (false). As with global search, normally 
        /// server-side processing scripts will not perform regular expression searching for performance reasons on large data sets, but it is technically 
        /// possible and at the discretion of your script.
        /// </summary>
        public bool IsRegex { get; set; }
    }
}






