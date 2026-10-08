namespace ReminderProX.Models;

public partial class OperatorOption : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsSelected { get; set; }
}