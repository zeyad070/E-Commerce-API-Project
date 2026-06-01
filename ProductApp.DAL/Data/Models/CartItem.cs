using System.ComponentModel.DataAnnotations.Schema;

namespace ProductApp.DAL
{
    public class CartItem
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Cart))]
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; } = null!;

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
