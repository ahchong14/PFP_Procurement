namespace PFP.Infrastructure.Options;

public sealed class AutoCountApiOptions
{
    public const string SectionName = "AutoCountApi";

    public string BaseUrl { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public int TimeoutSeconds { get; set; } = 30;
}
