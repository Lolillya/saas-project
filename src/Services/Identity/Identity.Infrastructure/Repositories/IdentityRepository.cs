using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly IdentityDbContext _context;

        public IdentityRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public Task<AspNetUsers?> GetUserByEmailAsync(string email, CancellationToken ct = default)
            => _context.Users.Include(u => u.Tenant).FirstOrDefaultAsync(u => u.Email == email, ct);

        public Task<AspNetUsers?> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
            => _context.Users.Include(u => u.Tenant).FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, ct);

        public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
            => _context.Users.AnyAsync(u => u.Email == email, ct);

        public Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default)
            => _context.Tenants.AnyAsync(t => t.Slug == slug, ct);

        public async Task<Tenants> AddTenantAsync(Tenants tenant, CancellationToken ct = default)
        {
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync(ct);
            return tenant;
        }

        public async Task AddUserAsync(AspNetUsers user, CancellationToken ct = default)
            => await _context.Users.AddAsync(user, ct);

        public Task UpdateUserAsync(AspNetUsers user, CancellationToken ct = default)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);

        public async Task AddUserTenantRoleAsync(UserTenantRoles userTenantRole, CancellationToken ct = default)
            => await _context.UserTenantRoles.AddAsync(userTenantRole, ct);

        public Task<string?> GetUserRoleAsync(int userId, int tenantId, CancellationToken ct = default)
            => _context.UserTenantRoles
                .Where(r => r.UserId == userId && r.TenantId == tenantId)
                .Include(r => r.Role)
                .Select(r => r.Role.RoleName)
                .FirstOrDefaultAsync(ct);

        public Task<Roles?> GetRoleByNameAsync(string roleName, CancellationToken ct = default)
            => _context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName, ct);
    }
}