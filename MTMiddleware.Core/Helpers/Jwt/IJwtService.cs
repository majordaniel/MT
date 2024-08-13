using System.Security.Claims;
using MTMiddleware.Core.Helpers.Autofac;

namespace MTMiddleware.Core.Helpers.Jwt
{
    public interface IJwtService : IAutoDependencyCore
    {
        public string? GenerateToken(Guid userId, Dictionary<string, object> claims);
        public string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}