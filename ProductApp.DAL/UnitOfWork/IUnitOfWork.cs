using Microsoft.EntityFrameworkCore.Storage;

namespace ProductApp.DAL
{
    public interface IUnitOfWork
    {
        IProductRepositories ProductRepositories { get; }
        ICategoryRepositories CategoryRepositories { get; }
        ICartRepositories CartRepositories { get; }
        IOrderRepositories OrderRepositories { get; }
        Task SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
