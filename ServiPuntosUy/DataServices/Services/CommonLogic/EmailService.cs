using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Implementación del servicio de email usando SMTP
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;

            _smtpServer = Environment.GetEnvironmentVariable("EMAIL_SMTP_SERVER") ?? throw new ArgumentNullException("EMAIL_SMTP_SERVER");
            _smtpPort = int.Parse(Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT") ?? "587");
            _smtpUsername = Environment.GetEnvironmentVariable("EMAIL_SMTP_USERNAME") ?? throw new ArgumentNullException("EMAIL_SMTP_USERNAME");
            _smtpPassword = Environment.GetEnvironmentVariable("EMAIL_SMTP_PASSWORD") ?? throw new ArgumentNullException("EMAIL_SMTP_PASSWORD");
            _fromEmail = Environment.GetEnvironmentVariable("EMAIL_FROM_EMAIL") ?? throw new ArgumentNullException("EMAIL_FROM_EMAIL");
            _fromName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME") ?? "ServiPuntosUY";
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(_smtpServer, _smtpPort)
                {
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = true
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(to);

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                // En producción, deberías usar un logger apropiado
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}
