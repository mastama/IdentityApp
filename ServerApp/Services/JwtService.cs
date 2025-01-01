using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using ServerApp.Models;

namespace ServerApp.Services;

public class JwtService
{
    // private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _jwtKey;
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        // _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        //
        // var key = _configuration["JWT:Key"];
        // if (string.IsNullOrEmpty(key))
        //     throw new InvalidOperationException("JWT key is empty or not configured");
        
        // jwtKey is used for both encripting and depcripting the JWT Token
        _jwtSettings = jwtSettings.Value;
        _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
    }

    public string GenerateToken(User user)
    {
        Log.Information("Process started: generating JWT token");
        
        var userClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? throw new ArgumentException("Email cannot be null, its required!")),
            new Claim(ClaimTypes.GivenName, user.FirstName!),
            new Claim(ClaimTypes.Surname, user.LastName!),
            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? throw new ArgumentException("PhoneNumber cannot be null, its required!")),
        };
        var credentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(userClaims),
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.ExpiresInDays),
            SigningCredentials = credentials,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var token = tokenHandler.CreateToken(tokenDescriptor);
            Log.Information("JWT token successfully generated");
            return tokenHandler.WriteToken(token);
        }
        catch (Exception e)
        {
            Log.Error(e, "JWT token generating failed");
            throw;
        }
    }
}