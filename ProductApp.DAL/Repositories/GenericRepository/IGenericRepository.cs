namespace ProductApp.DAL
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        void Insert(T entity);
        void Delete(T entity);
    }
}
