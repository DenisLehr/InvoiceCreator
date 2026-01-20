

namespace Data.Interfaces
{
    public interface IBaseRepository<T> 
    {
        Task<T> GetByIdAsync(string id);
        Task<List<T>> GetAllAsync();
        Task <bool> AddAsync(T entity);
        Task <bool> UpdateAsync(T entity);
        Task <bool> DeleteAsync(T entity);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();
    }
}
