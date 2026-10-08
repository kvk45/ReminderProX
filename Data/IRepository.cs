namespace ReminderProX.Data;

public interface IRepository<T> where T : class, new()
{
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetPagedAsync(int skip, int take);
    Task<T?> GetByIdAsync(int id);
    Task<int> SaveAsync(T entity);
    Task<int> DeleteAsync(T entity);
    Task<int> DeleteByIdAsync(int id);
}