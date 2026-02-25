using BookFace.Core.Application.DTO;
using BookFace.Core.Application.Interfaces.InterfacesService;
using BookFace.Core.Domain.Settings;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;


namespace BookFace.Infraestructure.Shared.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendAsync(EmailRequestDto emailRequestDto)
        {
            try
            {
                MimeMessage email = new()
                {
                    Sender = MailboxAddress.Parse(_mailSettings.EmailFrom),
                    Subject = emailRequestDto.Subject
                };

                // Asegúrate de tener al menos un destinatario
                if (!string.IsNullOrEmpty(emailRequestDto.To))
                {
                    email.To.Add(MailboxAddress.Parse(emailRequestDto.To));
                }

                if (emailRequestDto.ToRange != null)
                {
                    foreach (var toItem in emailRequestDto.ToRange)
                    {
                        if (!string.IsNullOrEmpty(toItem))
                        {
                            email.To.Add(MailboxAddress.Parse(toItem));
                        }
                    }
                }

                // Si no hay destinatarios, no tiene sentido enviar el email
                if (!email.To.Any())
                {
                    _logger.LogWarning("EmailService: No recipients specified for email with subject: {Subject}", emailRequestDto.Subject);
                    return;
                }

                BodyBuilder builder = new()
                {
                    HtmlBody = emailRequestDto.HtmlBody
                };
                email.Body = builder.ToMessageBody();

                using MailKit.Net.Smtp.SmtpClient smtpClient = new();
                await smtpClient.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_mailSettings.SmtpUser, _mailSettings.SmtpPass);
                await smtpClient.SendAsync(email);
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while sending email: {ExceptionMessage}", ex.Message);
                // Aquí podrías relanzar la excepción o devolver un resultado que indique fallo
                throw; // Es mejor relanzar para que la capa superior pueda manejarlo
            }
        }


    }
}