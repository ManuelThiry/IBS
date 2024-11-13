using System.Net.Mail;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace IBS_Europe.App.Pages.Shared.Email;

public class SmtpSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool EnableSsl { get; set; }
}

public class EmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
    {
        _smtpSettings = smtpSettings.Value;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, MimeEntity body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Nom de l'expéditeur", _smtpSettings.Username));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        // Utiliser directement le MimeEntity (body) passé en paramètre
        message.Body = body;

        using var client = new SmtpClient();
        try
        {
            _logger.LogInformation("Tentative d'envoi de l'e-mail...");
            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, _smtpSettings.EnableSsl);
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await client.SendAsync(message);
            _logger.LogInformation("E-mail envoyé avec succès à {toEmail}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi de l'e-mail.");
            return false;
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }

}
