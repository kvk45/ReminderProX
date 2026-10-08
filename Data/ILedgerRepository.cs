namespace ReminderProX.Data;

public interface ILedgerRepository : IRepository<LedgerItemModel>
{
    Task<List<LedgerItemModel>> GetAllLedgersSortedAsync();
    Task<List<LedgerItemModel>> GetPagedLedgersSortedAsync(int skip, int take);
    Task<List<LedgerItemModel>> SearchLedgersAsync(string query);
    Task<int> DeleteLedgerWithTransactionsAsync(int ledgerId);
    Task<int> ClearAllLedgersAsync();
}