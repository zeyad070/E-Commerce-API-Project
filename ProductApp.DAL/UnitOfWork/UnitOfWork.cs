using Microsoft.EntityFrameworkCore.Storage;

namespace ProductApp.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IProductRepositories ProductRepositories { get; }
        public ICategoryRepositories CategoryRepositories { get; }
        public ICartRepositories CartRepositories { get; }
        public IOrderRepositories OrderRepositories { get; }

        public UnitOfWork(
            AppDbContext context,
            IProductRepositories productRepositories,
            ICategoryRepositories categoryRepositories,
            ICartRepositories cartRepositories,
            IOrderRepositories orderRepositories)
        {
            _context = context;
            ProductRepositories = productRepositories;
            CategoryRepositories = categoryRepositories;
            CartRepositories = cartRepositories;
            OrderRepositories = orderRepositories;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
