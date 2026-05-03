using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Domain.Entities
{
    public class UserTenantRoles
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TenantId { get; set; }
        public DateTime AssignedAt { get; set; }

        public int RoleId { get; set; }

        [ForeignKey(nameof(UserId))]
        public AspNetUsers User { get; set; } = null!;
        [ForeignKey(nameof(RoleId))]
        public Roles Role { get; set; } = null!;
        [ForeignKey(nameof(TenantId))]
        public Tenants Tenant { get; set; } = null!;
    }
}
