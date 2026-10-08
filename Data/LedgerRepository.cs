namespace ReminderProX.Data;

public class LedgerRepository(DatabaseContext context) : Repository<LedgerItemModel>(context), ILedgerRepository
{
    public async Task<List<LedgerItemModel>> GetAllLedgersSortedAsync()
    {
        var db = await Context.GetConnectionAsync();
        return await db.Table<LedgerItemModel>()
            .OrderByDescending(l => l.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<LedgerItemModel>> GetPagedLedgersSortedAsync(int skip, int take)
    {
        var db = await Context.GetConnectionAsync();
        return await db.Table<LedgerItemModel>()
            .OrderByDescending(l => l.CreatedDateTime)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<LedgerItemModel>> SearchLedgersAsync(string query)
    {
        var db = await Context.GetConnectionAsync();
        var lowerQuery = query.ToLower();
        return await db.Table<LedgerItemModel>()
            .Where(l => l.UserName.ToLower().Contains(lowerQuery))
            .OrderByDescending(l => l.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<int> DeleteLedgerWithTransactionsAsync(int ledgerId)
    {
        var db = await Context.GetConnectionAsync();
        int rowsAffected = 0;

        await db.RunInTransactionAsync(conn =>
        {
            conn.Table<TransactionModel>().Where(t => t.LedgerId == ledgerId).Delete();
            rowsAffected = conn.Delete<LedgerItemModel>(ledgerId);
        });

        return rowsAffected;
    }

    public async Task<int> ClearAllLedgersAsync()
    {
        var db = await Context.GetConnectionAsync();
        return await db.DeleteAllAsync<LedgerItemModel>();
    }
}