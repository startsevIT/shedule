namespace Application.Services.Users;

public class SmtpSettings
{
    public required string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public required string SenderEmail { get; set; }
    public required string SenderPassword { get; set; }
}