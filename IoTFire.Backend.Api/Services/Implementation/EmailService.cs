using IoTFire.Backend.Api.Services.Interfaces;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;


namespace IoTFire.Backend.Api.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                // 1. Lire la config depuis appsettings.json
                var host = _config["EmailSettings:Host"];
                var port = int.Parse(_config["EmailSettings:Port"] ?? "587");
                var username = _config["EmailSettings:Username"];
                var password = _config["EmailSettings:Password"];
                var fromEmail = _config["EmailSettings:FromEmail"];
                var fromName = _config["EmailSettings:FromName"] ?? "Système Anti-Incendie";

                // 2. Construire le message
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                // 3. Corps HTML + fallback texte
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody,
                    TextBody = StripHtml(htmlBody) // fallback pour clients sans HTML
                };
                message.Body = bodyBuilder.ToMessageBody();

                // 4. Connexion SMTP et envoi
                using var client = new SmtpClient();

                // StartTls pour port 587, Ssl pour port 465
                var secureSocket = port == 465
                    ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTls;

                await client.ConnectAsync(host, port, secureSocket);
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email envoyé à {Email} | Sujet: {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Échec de l'envoi d'email à {Email}", toEmail);

                // On re-throw pour que l'appelant puisse gérer l'erreur
                throw new Exception($"Erreur lors de l'envoi de l'email : {ex.Message}", ex);
            }
        }


        private static string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return "";
            return System.Text.RegularExpressions.Regex
                .Replace(html, "<.*?>", string.Empty)
                .Trim();
        }
    }
}
