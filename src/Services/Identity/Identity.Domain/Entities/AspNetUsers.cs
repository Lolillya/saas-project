using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Domain.Entities
{
    public class AspNetUsers
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenants Tenant { get; set; } = null!;
    }
}
