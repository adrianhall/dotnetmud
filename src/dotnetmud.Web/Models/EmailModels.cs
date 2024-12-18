using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace dotnetmud.Web.Models;

/// <summary>
/// Model class for a single email address.
/// </summary>
public class EmailAddress(string address)
{
    [Required, EmailAddress]
    public string Address { get; set; } = address;

    [DisplayName]
    public string? DisplayName { get; set; }
}

/// <summary>
/// Model class for an email message.
/// </summary>
public class EmailMessage
{
    [Required, MinLength(1)]
    public IEnumerable<EmailAddress> Recipients { get; set; } = [];

    [Required, MinLength(1)]
    public required string Subject { get; set; }

    [Required, MinLength(1)]
    public required string TextContent { get; set; }

    [MinLength(12)]
    public string? HtmlContent { get; set; }
}

/// <summary>
/// Describes the result of an email send operation.
/// </summary>
public class EmailResult
{
    public bool Succeeded { get; set; }

    public int ResultCode { get; set; }

    public IEnumerable<string> Messages { get; set; } = [];
}
