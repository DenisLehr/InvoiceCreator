using Data.Configuration.SMTP;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Shared.Contracts.Interfaces;
using Shared.Contracts.Responses;
using Shared.Dtos.Email;

namespace Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _settings;

        public EmailService(SmtpSettings settings)
        {
            _settings = settings;
        }

        public async Task<BaseResponse<bool>> SendeNachrichtAnBuchhaltungAsync(EmailDto message)
        {
            message.EmpfaengerEmail = _settings.LexwareEmail;
            return await SendeNachrichtAsync(message);
        }

        public async Task<BaseResponse<bool>> SendeNachrichtAsync(EmailDto message)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_settings.DisplayName, _settings.From));
                email.To.Add(MailboxAddress.Parse(message.EmpfaengerEmail));
                email.Subject = message.Betreff;

                var builder = new BodyBuilder { HtmlBody = message.IsHtml ? message.Nachricht : null, TextBody = !message.IsHtml ? message.Nachricht : null };
                message.Anhang?.ForEach(a => builder.Attachments.Add(a.Dateiname, a.Inhalt));
                email.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                var options = _settings.UseStartTls
                    ? SecureSocketOptions.StartTls
                    : (_settings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.None);

                await client.ConnectAsync(_settings.Host, _settings.Port, options);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.SendAsync(email);
                await client.DisconnectAsync(true);

                return new BaseResponse<bool>
                {
                    Erfolg = true,
                    Hinweis = "Email erfolgreich versendet.",
                    Daten = false,
                    Zeitstempel = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {

                return new BaseResponse<bool>
                {
                    Erfolg = false,
                    Hinweis = "Emailversand fehlgeschlagen.",
                    Daten = false,
                    Zeitstempel = DateTime.UtcNow
                };
            }
        }
    }
}
