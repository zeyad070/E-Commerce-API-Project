using Microsoft.EntityFrameworkCore;

namespace ProductApp.DAL
{
    public class CartRepositories : GenericRepository<Cart>, ICartRepositories
    {
        public CartRepositories(AppDbContext context) : base(context)
        {
        }

        public async Task<Cart?> GetByUserIdAsync(string userId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetByUserIdWithItemsAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        public void DeleteCartItem(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
        }
    }
}
