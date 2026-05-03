using Identity.Domain.Entities;

namespace Identity.Application.Interfaces
{
    public interface IIdentityRepository
    {
        Task<AspNetUsers> GetUserByEmailAsync(string email, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
        Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);
        Task<Tenants> AddTenantAsync(Tenants tenant, CancellationToken ct = default);
        Task AddUserAsync(AspNetUsers user, CancellationToken ct = default);
        Task UpdateUserAsync(AspNetUsers user, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}