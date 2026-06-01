namespace ProductApp.DAL
{
    public interface IProductRepositories : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllWithCategoryAsync();
        Task<Product?> GetByIdWithCategoryAsync(int productId);
        Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedWithCategoryAsync(int? categoryId, string? name, int pageNumber, int pageSize);
    }
}


