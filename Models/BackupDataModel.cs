namespace ReminderProX.Models;

public class BackupDataModel
{
    public List<ReminderCardModel> Reminders { get; set; } = [];
    public List<LedgerItemModel> Ledgers { get; set; } = [];
    public List<TransactionModel> Transactions { get; set; } = [];
    public DateTime BackupDate { get; set; } = DateTime.Now;
}