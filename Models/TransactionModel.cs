namespace ReminderProX.Models;

[Table("Transactions")]
public class TransactionModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int LedgerId { get; set; }
    public decimal Amount { get; set; }
    public decimal PreviousBalance { get; set; }
    public string TransactionType { get; set; } = "Debit";
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedDateTime { get; set; } = DateTime.Now;
}