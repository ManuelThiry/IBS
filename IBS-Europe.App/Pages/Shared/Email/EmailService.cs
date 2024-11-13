using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body, IFormFile attachment = null);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, IFormFile attachment = null)
    {
        var smtpSettings = _config.GetSection("SmtpSettings");

        var smtpClient = new SmtpClient(smtpSettings["Host"])
        {
            Port = int.Parse(smtpSettings["Port"]),
            Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpSettings["Username"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        if (attachment != null)
        {
            using (var ms = new MemoryStream())
            {
                attachment.CopyTo(ms);
                var fileBytes = ms.ToArray();
                mailMessage.Attachments.Add(new Attachment(new MemoryStream(fileBytes), attachment.FileName));
            }
        }
        
        await smtpClient.SendMailAsync(mailMessage);

    }
}