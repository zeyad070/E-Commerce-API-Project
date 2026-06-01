using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductApp.DAL
{
    public class Cart : IAuditEntity
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = string.Empty;
        public virtual IdentityUser User { get; set; } = null!;
        public virtual ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
