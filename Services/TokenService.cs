using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public interface ITokenService
{
    public string GenerateAccessToken(LibraryUser user);
    public string GenerateRefreshToken();
    public DateTime GetRefreshExpiryDate();
    public DateTime GetAccessExpiryDate();
}

public class TokenService(
    IConfiguration _config,
    ILogger<TokenService> _logger
) : ITokenService
{
    public string GenerateAccessToken(LibraryUser user)
    {
        var jwt = _config.GetSection("Jwt");
        _logger.LogWarning($"user id: {user.Id}");
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.PhoneNumber, user.PhoneNumber)
        };

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["key"]));

        var credentials = new SigningCredentials(
            secretKey,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: jwt["issuer"],
            audience: jwt["audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["AccessTokenExpirationMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }

    public DateTime GetRefreshExpiryDate()
    {
        return DateTime.UtcNow.AddDays(int.Parse(_config.GetSection("jwt")["RefreshTokenExpirationDays"]));
    }

    public DateTime GetAccessExpiryDate()
    {
        return DateTime.UtcNow.AddMinutes(int.Parse(_config.GetSection("jwt")["AccessTokenExpirationMinutes"]));
    }

}
