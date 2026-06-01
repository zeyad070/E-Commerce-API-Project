namespace ProductApp.DAL
{
    public interface ICategoryRepositories : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetAllWithProductsAsync();
        Task<Category?> GetByIdWithProductAsync(int categoryId);
    }
}