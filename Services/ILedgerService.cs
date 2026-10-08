namespace ReminderProX.Services;

public interface ILedgerService
{
    Task<List<LedgerItemModel>> GetAllLedgersAsync();
    Task<List<LedgerItemModel>> GetPagedLedgersAsync(int skip, int take);
    Task<List<LedgerItemModel>> SearchLedgersAsync(string query);
    Task<LedgerItemModel?> GetLedgerByIdAsync(int id);
    Task<int> SaveLedgerAsync(LedgerItemModel ledger);
    Task<int> DeleteLedgerAsync(LedgerItemModel ledger);
    Task<int> DeleteLedgerWithTransactionsAsync(int ledgerId);
    Task<int> ClearAllLedgersAsync();
}