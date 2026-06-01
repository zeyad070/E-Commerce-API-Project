namespace ProductApp.DAL
{
    public interface ICartRepositories : IGenericRepository<Cart>
    {
        Task<Cart?> GetByUserIdAsync(string userId);
        Task<Cart?> GetByUserIdWithItemsAsync(string userId);
        Task<CartItem?> GetCartItemAsync(int cartId, int productId);
        void DeleteCartItem(CartItem cartItem);
    }
}
