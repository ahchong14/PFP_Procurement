using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PFP.Application.Abstractions.Services;
using PFP.Infrastructure.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PFP.Infrastructure.Identity;

internal sealed class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(
        IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateToken(
        int? userId,
        int? supplierId,
        string email,
        string? role)
    {
        if (userId is null && supplierId is null)
        {
            throw new ArgumentException(
                "Either UserId or SupplierId must be provided.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Email is required.");
        }

        if (string.IsNullOrWhiteSpace(role))
        {
            throw new ArgumentException(
                "Role is required.");
        }

        if (string.IsNullOrWhiteSpace(_options.SecretKey))
        {
            throw new InvalidOperationException(
                "JWT SecretKey is not configured.");
        }

        List<Claim> claims = new List<Claim>
        {
            new(
                ClaimTypes.Email,
                email),

            new(
                ClaimTypes.Role,
                role)
        };

        if (userId.HasValue)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId.Value.ToString()));
        }

        if (supplierId.HasValue)
        {
            claims.Add(
                new Claim(
                    "SupplierId",
                    supplierId.Value.ToString()));
        }

        SymmetricSecurityKey key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SecretKey));

        SigningCredentials credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(
            _options.ExpirationMinutes);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}