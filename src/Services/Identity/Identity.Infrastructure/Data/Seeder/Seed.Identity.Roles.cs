using Microsoft.EntityFrameworkCore;
using Identity.Domain.Entities;

namespace Identity.Infrastructure.Data.Seeder
{
    public class Role
    {
        public static void SeedIdentityRoles(ModelBuilder modelBuilder)
        {
            var roles = new List<Roles>
            {
                new Roles
                {
                    Id = 1,
                    RoleName = "Admin"
                },
                new Roles
                {
                    Id = 2,
                    RoleName = "Manager"
                },
                new Roles
                {
                    Id = 3,
                    RoleName = "Staff"
                }
            };
            modelBuilder.Entity<Roles>().HasData(roles);
        }
    }
}