using ProductApp.DAL;

namespace ProductApp.BLL
{
    public class CategoryManager : ICategoryManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryReadDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.CategoryRepositories.GetAllWithProductsAsync();
            return categories.Select(d => new CategoryReadDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                ImageUrl = d.ImageUrl,
                ProductCount = d.Products.Count()
            }).ToList();
        }

        public async Task<CategoryReadDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepositories.GetByIdWithProductAsync(id);
            if (category == null) return null;

            return new CategoryReadDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                ProductCount = category.Products.Count(),
            };
        }

        public async Task<int> CreateCategoryAsync(CategoryCreateDto categoryCreateDto)
        {
            var category = new Category
            {
                Name = categoryCreateDto.Name,
                Description = categoryCreateDto.Description,
            };
            _unitOfWork.CategoryRepositories.Insert(category);
            await _unitOfWork.SaveChangesAsync();
            return category.Id;
        }

        public async Task UpdateCategoryAsync(CategoryEditDto categoryEditDto)
        {
            var category = await _unitOfWork.CategoryRepositories.GetByIdWithProductAsync(categoryEditDto.Id);
            if (category == null) return;

            category.Name = categoryEditDto.Name;
            category.Description = categoryEditDto.Description;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepositories.GetByIdAsync(id);
            if (category == null) return;

            _unitOfWork.CategoryRepositories.Delete(category);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
