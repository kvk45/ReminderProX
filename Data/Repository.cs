namespace ReminderProX.Data;

public class Repository<T>(DatabaseContext context) : IRepository<T> where T : class, new()
{
    protected readonly DatabaseContext Context = context;
    private static readonly PropertyInfo? IdProperty = typeof(T).GetProperty("Id");

    public async Task<List<T>> GetAllAsync()
    {
        var db = await Context.GetConnectionAsync();
        return await db.Table<T>().ToListAsync();
    }

    public async Task<List<T>> GetPagedAsync(int skip, int take)
    {
        var db = await Context.GetConnectionAsync();
        return await db.Table<T>().Skip(skip).Take(take).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var db = await Context.GetConnectionAsync();
        return await db.FindAsync<T>(id);
    }

    public async Task<int> SaveAsync(T entity)
    {
        var db = await Context.GetConnectionAsync();
        if (IdProperty != null)
        {
            var idValue = (int)(IdProperty.GetValue(entity) ?? 0);
            if (idValue != 0)
            {
                return await db.UpdateAsync(entity);
            }
        }
        return await db.InsertAsync(entity);
    }

    public async Task<int> DeleteAsync(T entity)
    {
        var db = await Context.GetConnectionAsync();
        return await db.DeleteAsync(entity);
    }

    public async Task<int> DeleteByIdAsync(int id)
    {
        var db = await Context.GetConnectionAsync();
        return await db.DeleteAsync<T>(id);
    }
}