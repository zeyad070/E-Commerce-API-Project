using ProductApp.Common.Utilities;

namespace ProductApp.BLL
{
    public interface IProductManager
    {
        Task<PaginationResponse<ProductReadDto>> GetAllProductsAsync(int? categoryId, string? name, int pageNumber, int pageSize);
        Task<ProductReadDto?> GetProductByIdAsync(int id);
        Task<ProductEditDto?> GetProductByIdEditAsync(int id);
        Task<int> CreateProductAsync(ProductCreateDto productCreateDto);
        Task UpdateProductAsync(ProductEditDto productEditDto);
        Task DeleteProductAsync(int id);
        Task<List<CategoryReadDto>?> GetCategoryListAsync();
    }
}
