namespace ReminderProX.Models;

public partial class UserModel : ObservableObject
{
    [ObservableProperty]
    public partial string UserId { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string DisplayName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Email { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsSignedIn { get; set; }
}