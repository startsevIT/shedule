using System.Net;
using System.Net.Mail;

namespace Application.Services.Users;

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