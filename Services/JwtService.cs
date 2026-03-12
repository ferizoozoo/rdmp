using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Services;

public interface IJwtService
{
    string GenerateAccessToken(int userId);
    bool ValidateAccessToken(string token, out int userId);
    string GenerateRefreshToken();
    bool ValidateRefreshToken(string token);
}

public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly int _accessTokenExpirationMinutes;
    private readonly int _refreshTokenExpirationMinutes;

    // In-memory store for valid refresh tokens: token -> expiry
    private static readonly Dictionary<string, DateTime> _refreshTokens = new();

    public JwtService()
    {
        _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "default_secret_key";
        _accessTokenExpirationMinutes = int.TryParse(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES"), out var accessExp) ? accessExp : 15;
        _refreshTokenExpirationMinutes = int.TryParse(Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRATION_MINUTES"), out var refreshExp) ? refreshExp : 43200;
    }

    public string GenerateAccessToken(int userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: [new Claim("userId", userId.ToString())],
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateAccessToken(string token, out int userId)
    {
        userId = 0;
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            var claim = principal.FindFirst("userId")?.Value;
            return claim != null && int.TryParse(claim, out userId);
        }
        catch
        {
            return false;
        }
    }

    public string GenerateRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        _refreshTokens[token] = DateTime.UtcNow.AddMinutes(_refreshTokenExpirationMinutes);
        return token;
    }

    public bool ValidateRefreshToken(string token)
    {
        if (_refreshTokens.TryGetValue(token, out var expiry) && expiry > DateTime.UtcNow)
            return true;

        _refreshTokens.Remove(token); // clean up expired
        return false;
    }
}
