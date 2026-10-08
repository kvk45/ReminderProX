namespace ReminderProX.Data;

public class ReminderRepository(DatabaseContext context) : Repository<ReminderCardModel>(context), IReminderRepository
{
    public async Task<List<ReminderCardModel>> GetActiveRemindersSortedAsync()
    {
        var db = await Context.GetConnectionAsync();
        return await db.Table<ReminderCardModel>()
            .Where(r => r.IsActive)
            .OrderBy(r => r.DueDate)
            .ToListAsync();
    }

    public async Task<List<ReminderCardModel>> GetPagedActiveRemindersSortedAsync(int skip, int take)
    {
        var db = await Context.GetConnectionAsync();
        return await db.Table<ReminderCardModel>()
            .Where(r => r.IsActive)
            .OrderBy(r => r.DueDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<ReminderCardModel>> SearchRemindersAsync(string query)
    {
        var db = await Context.GetConnectionAsync();
        var lowerQuery = query.ToLower();
        return await db.Table<ReminderCardModel>()
            .Where(r => r.IsActive &&
                       (r.Name.ToLower().Contains(lowerQuery) ||
                        r.Number.ToLower().Contains(lowerQuery) ||
                        r.OperatorName.ToLower().Contains(lowerQuery)))
            .OrderBy(r => r.DueDate)
            .ToListAsync();
    }

    public async Task<int> ClearAllRemindersAsync()
    {
        var db = await Context.GetConnectionAsync();
        return await db.DeleteAllAsync<ReminderCardModel>();
    }
}