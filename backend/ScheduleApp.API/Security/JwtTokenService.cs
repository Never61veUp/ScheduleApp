using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MiniApp.Server.Security;

namespace ScheduleApp.API.Security;

public sealed class JwtTokenService
{
    public string CreateToken(TelegramWebAppUser user, IConfiguration config)
    {
        var issuer = config["JWT_ISSUER"] ?? "MiniApp";
        var audience = config["JWT_AUDIENCE"] ?? "MiniApp";
        var key = config["JWT_KEY"];
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Missing JWT signing key. Set environment variable JWT_KEY.");

        var role = ResolveRole(user, config);

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("tg:id", user.Id.ToString()),
            new(ClaimTypes.Role, role),
        };

        if (!string.IsNullOrWhiteSpace(user.Username)) claims.Add(new("tg:username", user.Username));
        if (!string.IsNullOrWhiteSpace(user.FirstName)) claims.Add(new("tg:first_name", user.FirstName));
        if (!string.IsNullOrWhiteSpace(user.LastName)) claims.Add(new("tg:last_name", user.LastName));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow.AddSeconds(-5),
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string ResolveRole(TelegramWebAppUser user, IConfiguration config)
    {
        // Default role
        const string clientRole = "client";
        const string masterRole = "master";

        // Comma/space-separated list of Telegram user IDs that should be masters.
        // Example: MASTER_TG_IDS=12345,67890
        var masters = config["MASTER_TG_IDS"];
        if (string.IsNullOrWhiteSpace(masters))
            return clientRole;

        var parts = masters
            .Split(new[] { ',', ' ', ';', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var p in parts)
        {
            if (long.TryParse(p, out var id) && id == user.Id)
                return masterRole;
        }

        return clientRole;
    }
}

