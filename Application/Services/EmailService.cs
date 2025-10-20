using Core.Interfaces.IServices.IEmailServices;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly ISendGridClient _client;
        private readonly IConfiguration _config;

        public SendGridEmailService(ISendGridClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent, string plainTextContent = null)
        {
            var fromEmail = _config["SendGrid:FromEmail"];
            var fromName = _config["SendGrid:FromName"];

            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent ?? "", htmlContent);

            var response = await _client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync();
                throw new Exception($"SendGrid failed: {response.StatusCode} - {body}");
            }
        }

        public async Task<bool> PingAsync()
        {
            try
            {
                // Try a no-op API call — SendGrid requires valid API key.
                var response = await _client.RequestAsync(
                    method: SendGridClient.Method.GET,
                    urlPath: "user/profile"
                );

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
