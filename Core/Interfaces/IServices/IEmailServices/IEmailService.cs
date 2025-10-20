namespace Core.Interfaces.IServices.IEmailServices;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlContent, string plainTextContent = null);
    Task<bool> PingAsync();
}
