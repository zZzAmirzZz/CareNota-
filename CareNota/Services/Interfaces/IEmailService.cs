namespace CareNota.Services.Interfaces;

public interface IEmailService
{
    Task SendAsync(string toEmail, string toName, string subject, string htmlBody);
}