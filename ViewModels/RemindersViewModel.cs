namespace ReminderProX.ViewModels;

public partial class RemindersViewModel : BaseViewModel
{
    private readonly IReminderService _reminderService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    public partial ObservableCollection<ReminderCardModel> Reminders { get; set; } = [];

    [ObservableProperty]
    public partial string NewTitle { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewDescription { get; set; } = string.Empty;

    public RemindersViewModel(IReminderService reminderService, IDialogService dialogService)
    {
        _reminderService = reminderService;
        _dialogService = dialogService;
        Title = "Reminders";

        WeakReferenceMessenger.Default.Register<DataRestoredMessage>(this, async (r, m) =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await LoadRemindersAsync();
            });
        });
    }

    public async Task LoadRemindersAsync()
    {
        var list = await _reminderService.GetAllRemindersAsync();
        Reminders = new ObservableCollection<ReminderCardModel>(list);
    }

    [RelayCommand]
    private async Task AddReminderAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTitle))
        {
            await _dialogService.ShowAlertAsync("Validation", "Please enter a reminder title.");
            return;
        }

        var newReminder = new ReminderCardModel
        {
            Name = NewTitle.Trim(),
            RechargedAt = DateTime.Now,
            DueDate = DateTime.Now.AddDays(28),
            IsActive = true
        };

        await _reminderService.SaveReminderAsync(newReminder);
        Reminders.Add(newReminder);
        NewTitle = string.Empty;
        NewDescription = string.Empty;
    }

    [RelayCommand]
    private async Task DeleteReminderAsync(ReminderCardModel reminder)
    {
        if (reminder != null)
        {
            bool confirm = await _dialogService.ShowConfirmationAsync("Confirm Delete", "Delete this reminder?");
            if (confirm)
            {
                await _reminderService.DeleteReminderAsync(reminder);
                Reminders.Remove(reminder);
            }
        }
    }
}