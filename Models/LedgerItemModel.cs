namespace ReminderProX.Models;

[Table("LedgerItems")]
public class LedgerItemModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    public decimal Balance { get; set; } = 0m;
}