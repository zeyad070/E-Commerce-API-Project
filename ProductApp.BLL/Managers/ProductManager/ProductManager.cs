using ProductApp.Common.Utilities;
using ProductApp.DAL;

namespace ProductApp.BLL
{
    public class ProductManager : IProductManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginationResponse<ProductReadDto>> GetAllProductsAsync(int? categoryId, string? name, int pageNumber, int pageSize)
        {
            var (items, totalCount) = await _unitOfWork.ProductRepositories.GetPagedWithCategoryAsync(categoryId, name, pageNumber, pageSize);

            return new PaginationResponse<ProductReadDto>
            {
                Items = items.Select(p => new ProductReadDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId,
                    Category = p.Category.Name
                }).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ProductReadDto?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.ProductRepositories.GetByIdWithCategoryAsync(id);
            if (product == null) return null;

            return new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                Category = product.Category.Name
            };
        }

        public async Task<int> CreateProductAsync(ProductCreateDto productCreateDto)
        {
            var product = new Product
            {
                Name = productCreateDto.Name,
                Description = productCreateDto.Description,
                Price = productCreateDto.Price,
                StockQuantity = productCreateDto.StockQuantity,
                CategoryId = productCreateDto.CategoryId,
            };
            _unitOfWork.ProductRepositories.Insert(product);
            await _unitOfWork.SaveChangesAsync();
            return product.Id;
        }

        public async Task UpdateProductAsync(ProductEditDto productEditDto)
        {
            var product = await _unitOfWork.ProductRepositories.GetByIdAsync(productEditDto.Id);
            if (product == null) return;

            if (productEditDto.Name != null) product.Name = productEditDto.Name;
            if (productEditDto.Description != null) product.Description = productEditDto.Description;
            if (productEditDto.Price != null) product.Price = productEditDto.Price.Value;
            if (productEditDto.StockQuantity != null) product.StockQuantity = productEditDto.StockQuantity.Value;
            if (productEditDto.CategoryId != null) product.CategoryId = productEditDto.CategoryId.Value;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.ProductRepositories.GetByIdAsync(id);
            if (product == null) return;
            _unitOfWork.ProductRepositories.Delete(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<CategoryReadDto>?> GetCategoryListAsync()
        {
            var categories = await _unitOfWork.CategoryRepositories.GetAllAsync();

            return categories.Select(c => new CategoryReadDto
            {
                Id = c.Id,
                Name = c.Name,
            }).ToList();
        }

        public async Task<ProductEditDto?> GetProductByIdEditAsync(int id)
        {
            var product = await _unitOfWork.ProductRepositories.GetByIdWithCategoryAsync(id);

            if (product == null)
                return null;

            return new ProductEditDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
            };
        }
    }
}
