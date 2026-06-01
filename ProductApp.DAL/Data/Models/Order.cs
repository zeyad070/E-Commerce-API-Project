using Microsoft.AspNetCore.Identity;
using ProductApp.Common.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductApp.DAL
{
    public class Order : IAuditEntity
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = string.Empty;
        public virtual IdentityUser User { get; set; } = null!;

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = OrderStatus.Pending.ToString();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
