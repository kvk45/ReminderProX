namespace ReminderProX.Services;

public interface ITransactionService
{
    Task<List<TransactionModel>> GetTransactionsByLedgerIdAsync(int ledgerId);
    Task<List<TransactionModel>> GetPagedTransactionsByLedgerIdAsync(int ledgerId, int skip, int take);
    Task<List<TransactionModel>> GetAllTransactionsAsync();
    Task<int> SaveTransactionAsync(TransactionModel transaction);
    Task<int> DeleteTransactionAsync(TransactionModel transaction);
    Task<int> ClearAllTransactionsAsync();
}