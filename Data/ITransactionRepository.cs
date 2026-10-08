namespace ReminderProX.Data;

public interface ITransactionRepository : IRepository<TransactionModel>
{
    Task<List<TransactionModel>> GetTransactionsByLedgerIdAsync(int ledgerId);
    Task<List<TransactionModel>> GetPagedTransactionsByLedgerIdAsync(int ledgerId, int skip, int take);
    Task<int> DeleteTransactionsByLedgerIdAsync(int ledgerId);
    Task<int> ClearAllTransactionsAsync();
}