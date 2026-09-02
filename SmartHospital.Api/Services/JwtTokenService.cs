using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SmartHospital.Domain.Entities;
using Claim = System.Security.Claims.Claim;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace SmartHospital.Api.Services;

public class JwtTokenService
{
    private readonly IConfiguration _config;
    public JwtTokenService(IConfiguration config) => _config = config;

    public string GenerateToken(StaffUser user, IList<string> roles)
    {
        var key = _config["Jwt:Key"] ?? "DEV_ONLY_32_CHAR_SECRET_KEY_CHANGE_IN_PROD_12345";
        var issuer = _config["Jwt:Issuer"] ?? "SmartHospital";
        var audience = _config["Jwt:Audience"] ?? "SmartHospital.Client";
        var expiryMin = int.TryParse(_config["Jwt:ExpiryMinutes"], out var m) ? m : 480;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? user.FullName),
            new(ClaimTypes.Email, user.Email ?? ""),
            new("fullName", user.FullName),
            new("role", user.Role.ToString()),
        };
        foreach (var r in roles) claims.Add(new Claim(ClaimTypes.Role, r));

        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddMinutes(expiryMin), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
