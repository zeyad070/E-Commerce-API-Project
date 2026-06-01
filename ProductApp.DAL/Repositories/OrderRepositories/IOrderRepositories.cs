namespace ProductApp.DAL
{
    public interface IOrderRepositories : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task<Order?> GetByIdWithItemsAsync(int orderId);
    }
}
