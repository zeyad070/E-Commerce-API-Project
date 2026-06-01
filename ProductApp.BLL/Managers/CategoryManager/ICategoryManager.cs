namespace ProductApp.BLL
{
    public interface ICategoryManager
    {
        Task<List<CategoryReadDto>> GetAllCategoriesAsync();
        Task<CategoryReadDto?> GetCategoryByIdAsync(int id);
        Task<int> CreateCategoryAsync(CategoryCreateDto categoryCreateDto);
        Task UpdateCategoryAsync(CategoryEditDto categoryEditDto);
        Task DeleteCategoryAsync(int id);
    }
}
