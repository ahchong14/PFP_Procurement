namespace PFP.Application.Abstractions.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken);
    }
}
