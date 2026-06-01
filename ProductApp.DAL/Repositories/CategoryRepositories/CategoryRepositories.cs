using Microsoft.EntityFrameworkCore;

namespace ProductApp.DAL
{
    public class CategoryRepositories : GenericRepository<Category>, ICategoryRepositories
    {
        public CategoryRepositories(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetAllWithProductsAsync()
        {
            return await _context.Categories.Include(c => c.Products).AsNoTracking().ToListAsync();
        }

        public async Task<Category?> GetByIdWithProductAsync(int categoryId)
        {
            return await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == categoryId);
        }
    }
}
