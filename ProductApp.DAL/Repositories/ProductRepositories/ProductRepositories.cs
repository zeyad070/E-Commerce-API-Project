using Microsoft.EntityFrameworkCore;

namespace ProductApp.DAL
{
    public class ProductRepositories : GenericRepository<Product>, IProductRepositories
    {
        public ProductRepositories(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
        {
            return await _context.Products.Include(p => p.Category).AsNoTracking().ToListAsync();
        }

        public async Task<Product?> GetByIdWithCategoryAsync(int productId)
        {
            return await _context.Products.Include(p => p.Category).AsNoTracking().FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedWithCategoryAsync(int? categoryId, string? name, int pageNumber, int pageSize)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsNoTracking();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(p => p.Name.Contains(name));

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
