namespace Application.Services.Users;

public class JwtSettings
{
    public string Secret { get; init; } = null!;
    public int ExpirationMinutes { get; init; }
};