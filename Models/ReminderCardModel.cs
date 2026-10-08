namespace ReminderProX.Models;

[Table("Reminders")]
public class ReminderCardModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Category { get; set; } = "Mobile";
    public string Name { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public string OperatorIcon { get; set; } = "operator_jio.png";
    public decimal Amount { get; set; } = 0m;
    public DateTime RechargedAt { get; set; } = DateTime.Now;

    [Indexed]
    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(28);
    public int ValidityDays { get; set; } = 28;

    [Indexed]
    public bool IsActive { get; set; } = true;
}