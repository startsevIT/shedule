using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Application.Services.Users;

public class EmailService(IOptions<SmtpSettings> smtpSettingsOptions)
{
    private readonly SmtpSettings _smtpSettings = smtpSettingsOptions.Value;
    private readonly SmtpClientWrapper _smtpClientWrapper = new(smtpSettingsOptions.Value.SmtpServer, smtpSettingsOptions.Value.SmtpPort);

    public async Task<bool> SendAsync(string? message, string to)
    {
        if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(to))
            throw new Exception("Попытка отправить пустое сообщение на пустую почту");

        try
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, "Студенческий сервис"),
                Subject = "Ваши учётные данные для доступа к сервису",
                IsBodyHtml = true,
                Body = message,
                To = { to }
            };

            _smtpClientWrapper.SetCredentials(_smtpSettings.SenderEmail, _smtpSettings.SenderPassword);

            await _smtpClientWrapper.SendMailAsync(mailMessage);
            return true;
        }
        catch
        {
            throw new Exception($"Не получилось отправить сообщение на почту {to}");
        }
    }
}

public class SmtpClientWrapper(string host, int port)
{
    private readonly SmtpClient _client = new(host, port)
    {
        EnableSsl = true
    };

    public void SetCredentials(string username, string password) =>
        _client.Credentials = new NetworkCredential(username, password);

    public Task SendMailAsync(MailMessage message) =>
        _client.SendMailAsync(message);
}


public class SmtpSettings
{
    public required string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public required string SenderEmail { get; set; }
    public required string SenderPassword { get; set; }
}