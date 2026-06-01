namespace ProductApp.BLL
{
    public interface ICartManager
    {
        Task<CartResponseDto?> GetCartAsync(string userId);
        Task<CartResponseDto> AddToCartAsync(string userId, AddToCartDto dto);
        Task<CartResponseDto> UpdateCartAsync(string userId, UpdateCartDto dto);
        Task RemoveFromCartAsync(string userId, int productId);
    }
}
