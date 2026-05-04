using Microsoft.EntityFrameworkCore;
using Identity.Domain.Entities;

namespace Identity.Infrastructure.Data.Seeder
{
    public class Plans
    {
        public static void SeedIdentityPlans(ModelBuilder modelBuilder)
        {
            var Plan = new List<Plan>
            {
                new Plan
                {
                    PlanId = 1,
                    PlanName = "Basic"
                },
                new Plan
                {
                    PlanId = 2,
                    PlanName = "Pro"
                },
                new Plan
                {
                    PlanId = 3,
                    PlanName = "Enterprise"
                }
            };
            modelBuilder.Entity<Plan>().HasData(Plan);

        }
    }
}