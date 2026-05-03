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

        public async Task UpdateUserAsync(AspNetUsers user, CancellationToken ct = default)
            => await Task.FromResult(_context.Users.Update(user));

        public async Task SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);
    }
}