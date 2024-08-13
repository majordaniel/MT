using System.Security.Claims;

namespace MTMiddleware.Core.Auth
{
    public interface IJwtService
    {
        public string? GenerateToken(string userId, Dictionary<string, object> claims);
        public string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        public int? ValidateToken(string token);
        public IDictionary<string, string?>? DecryptToken(string token);
    }
}