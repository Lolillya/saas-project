using Identity.Domain.Entities;

namespace Identity.Application.Services
{

    public record TokenResult(string Token, DateTime ExpiresAt);

    public interface ITokenServices
    {
        // string GenerateToken(AspNetUsers user, string tenantSlug, string role);
        string GenerateRefreshToken();
        TokenResult GenerateToken(AspNetUsers user, string tenantSlug, string role);

    }
}