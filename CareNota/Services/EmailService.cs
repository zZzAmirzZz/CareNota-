//using CareNota.Models;
//using CareNota.Services.Interfaces;
//using MailKit.Net.Smtp;
//using MailKit.Security;
//using Microsoft.Extensions.Options;
//using MimeKit;

//namespace CareNota.Services;

//public class EmailService : IEmailService
//{
//    private readonly EmailSettings _settings;

//    public EmailService(IOptions<EmailSettings> options)
//    {
//        _settings = options.Value;
//    }

//    public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
//    {
//        var message = new MimeMessage();
//        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
//        message.To.Add(new MailboxAddress(toName, toEmail));
//        message.Subject = subject;
//        message.Body = new TextPart("html") { Text = htmlBody };

//        using var client = new SmtpClient();
//        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
//        await client.AuthenticateAsync(_settings.SenderEmail, _settings.AppPassword);
//        await client.SendAsync(message);
//        await client.DisconnectAsync(true);
//    }
//}