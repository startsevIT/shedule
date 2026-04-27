using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Users;

public class TokenService(IOptions<JwtSettings> jwtSettings)
{
    private readonly JwtSettings _settings = jwtSettings.Value;
    private readonly SymmetricSecurityKey _signingKey = new(Encoding.UTF8.GetBytes(jwtSettings.Value.Secret));

    public string CreateToken(Guid id, string[] roles)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, id.ToString())
        ];

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.ToString())));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public class JwtSettings
{
    public string Secret { get; init; } = null!;
    public int ExpirationMinutes { get; init; }
};