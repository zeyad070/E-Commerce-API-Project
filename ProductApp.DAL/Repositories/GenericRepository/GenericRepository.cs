using Microsoft.EntityFrameworkCore;

namespace ProductApp.DAL
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public void Insert(T entity)
        {
            _context.Add(entity);
        }
        public void Delete(T entity)
        {
            _context.Remove(entity);
        }
    }
}
