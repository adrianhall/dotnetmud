namespace dotnetmud.Web.Models;

public class OAuthProviderOptions
{
    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public bool Enabled { get => !string.IsNullOrEmpty(ClientId) && !string.IsNullOrEmpty(ClientSecret); }
}
