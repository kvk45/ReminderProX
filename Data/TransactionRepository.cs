namespace ReminderProX.Data;

public class TransactionRepository(DatabaseContext context) : Repository<TransactionModel>(context), ITransactionRepository
{
    public async Task<List<TransactionModel>> GetTransactionsByLedgerIdAsync(int ledgerId)
    {
        var db = await Context.GetConnectionAsync();
        return await db.Table<TransactionModel>()
            .Where(t => t.LedgerId == ledgerId)
            .OrderByDescending(t => t.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<TransactionModel>> GetPagedTransactionsByLedgerIdAsync(int ledgerId, int skip, int take)
    {
        var db = await Context.GetConnectionAsync();
        return await db.Table<TransactionModel>()
            .Where(t => t.LedgerId == ledgerId)
            .OrderByDescending(t => t.CreatedDateTime)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> DeleteTransactionsByLedgerIdAsync(int ledgerId)
    {
        var db = await Context.GetConnectionAsync();
        return await db.Table<TransactionModel>()
            .Where(t => t.LedgerId == ledgerId)
            .DeleteAsync();
    }

    public async Task<int> ClearAllTransactionsAsync()
    {
        var db = await Context.GetConnectionAsync();
        return await db.DeleteAllAsync<TransactionModel>();
    }
}