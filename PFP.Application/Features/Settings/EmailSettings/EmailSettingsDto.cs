namespace PFP.Application.Features.Settings.EmailSettings
{
    // Password is deliberately never included - the settings page shows everything
    // except the current secret, same as any "change password" style form.
    public sealed record EmailSettingsDto(
        string Host,
        int Port,
        string Username,
        string FromEmail,
        string FromName);
}
