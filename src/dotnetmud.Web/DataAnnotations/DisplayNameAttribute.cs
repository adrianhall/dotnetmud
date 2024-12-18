using System.ComponentModel.DataAnnotations;

namespace dotnetmud.Web.DataAnnotations;

/// <summary>
/// A data annotation that provides a display name validation.
/// </summary>
public class DisplayNameAttribute() : RegularExpressionAttribute(RegularExpression)
{
    private const string RegularExpression = @"^[a-zA-Z0-9- \.]+$";
}
