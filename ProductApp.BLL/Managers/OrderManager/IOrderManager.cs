namespace ProductApp.BLL
{
    public interface IOrderManager
    {
        Task<OrderDetailsDto> CreateOrderAsync(string userId);
        Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId);
        Task<OrderDetailsDto?> GetOrderByIdAsync(int orderId, string userId);
    }
}
