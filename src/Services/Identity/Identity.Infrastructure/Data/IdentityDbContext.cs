using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Data
{
    public class IdentityDbContext : DbContext
    {

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        public DbSet<AspNetUsers> Users { get; set; }
        public DbSet<Tenants> Tenants { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<UserTenantRoles> UserTenantRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.CompanyName).HasMaxLength(256);

                entity.HasOne(u => u.Tenant)
                    .WithMany()
                    .HasForeignKey(u => u.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Tenants>(entity =>
            {
                entity.HasKey(t => t.TenantId);
                entity.HasIndex(t => t.Slug).IsUnique();
                entity.Property(t => t.Slug).IsRequired().HasMaxLength(100);
                entity.Property(t => t.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(t => t.LastName).IsRequired().HasMaxLength(100);

                entity.HasOne(t => t.Plan)
                    .WithMany()
                    .HasForeignKey(t => t.PlanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.HasIndex(r => r.RoleName).IsUnique();
                entity.Property(r => r.RoleName).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Plan>(entity =>
            {
                entity.HasKey(r => r.PlanId);
                entity.Property(p => p.PlanName).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<UserTenantRoles>(entity =>
            {
                entity.HasKey(utr => utr.Id);

                entity.HasOne(utr => utr.User)
                    .WithMany()
                    .HasForeignKey(utr => utr.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(utr => utr.Tenant)
                    .WithMany()
                    .HasForeignKey(utr => utr.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(utr => utr.Role)
                    .WithMany()
                    .HasForeignKey(utr => utr.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

    }
}