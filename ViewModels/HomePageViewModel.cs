namespace ReminderProX.ViewModels;

public partial class HomePageViewModel : BaseViewModel
{
    private readonly IReminderService _reminderService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly IAppNotificationService _notificationService;

    private const int PageSize = 15;
    private int _skipCount = 0;
    private bool _hasMoreItems = true;
    private bool _isLoadingMore = false;

    [ObservableProperty]
    public partial ObservableCollection<ReminderCardModel> AllReminders { get; set; } = [];

    [ObservableProperty]
    public partial ObservableCollection<ReminderCardModel> FilteredReminders { get; set; } = [];

    [ObservableProperty]
    public partial string SearchQuery { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }

    public HomePageViewModel(
        IReminderService reminderService,
        IDialogService dialogService,
        INavigationService navigationService,
        IAppNotificationService notificationService)
    {
        _reminderService = reminderService;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _notificationService = notificationService;
        Title = "ReminderProX";

        WeakReferenceMessenger.Default.Register<DataRestoredMessage>(this, async (r, m) =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await LoadRemindersAsync();
            });
        });
    }

    async partial void OnSearchQueryChanged(string value) => await FilterRemindersAsync();

    public async Task LoadRemindersAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            _skipCount = 0;
            _hasMoreItems = true;

            var list = await _reminderService.GetPagedRemindersAsync(_skipCount, PageSize);
            if (list != null && list.Count > 0)
            {
                var sortedList = list.OrderBy(r => (r.DueDate.Date - DateTime.Now.Date).Days).ToList();

                AllReminders = new ObservableCollection<ReminderCardModel>(sortedList);
                FilteredReminders = new ObservableCollection<ReminderCardModel>(sortedList);

                _skipCount += list.Count;
                if (list.Count < PageSize)
                {
                    _hasMoreItems = false;
                }

                // UI-ஐ தாமதப்படுத்தாமல் பின்னணியில் நோட்டிபிகேஷன் ஷெட்யூல் செய்யப்படுகிறது
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.CheckAndScheduleAllAsync(list);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Notification Schedule Error: {ex.Message}");
                    }
                });
            }
            else
            {
                AllReminders.Clear();
                FilteredReminders.Clear();
                _hasMoreItems = false;
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync("Error", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task LoadMoreRemindersAsync()
    {
        if (_isLoadingMore || !_hasMoreItems || !string.IsNullOrWhiteSpace(SearchQuery)) return;

        _isLoadingMore = true;
        try
        {
            var list = await _reminderService.GetPagedRemindersAsync(_skipCount, PageSize);
            if (list != null && list.Count > 0)
            {
                var sortedList = list.OrderBy(r => (r.DueDate.Date - DateTime.Now.Date).Days).ToList();
                foreach (var item in sortedList)
                {
                    AllReminders.Add(item);
                    FilteredReminders.Add(item);
                }
                _skipCount += list.Count;
                if (list.Count < PageSize)
                {
                    _hasMoreItems = false;
                }

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.CheckAndScheduleAllAsync(list);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Notification Schedule Error: {ex.Message}");
                    }
                });
            }
            else
            {
                _hasMoreItems = false;
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync("Error", ex.Message);
        }
        finally
        {
            _isLoadingMore = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        SearchQuery = string.Empty;
        await LoadRemindersAsync();
        IsRefreshing = false;
    }

    [RelayCommand]
    private async Task SearchAsync() => await FilterRemindersAsync();

    private async Task FilterRemindersAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            var sorted = AllReminders.OrderBy(r => (r.DueDate.Date - DateTime.Now.Date).Days).ToList();
            FilteredReminders = new ObservableCollection<ReminderCardModel>(sorted);
        }
        else
        {
            var results = await _reminderService.SearchRemindersAsync(SearchQuery.Trim());
            var sorted = results.OrderBy(r => (r.DueDate.Date - DateTime.Now.Date).Days).ToList();
            FilteredReminders = new ObservableCollection<ReminderCardModel>(sorted);
        }
    }

    [RelayCommand]
    private async Task OpenAddOptionsAsync()
    {
        string? action = await _dialogService.ShowActionSheetAsync("Select Type", "Cancel", null, "📱 Mobile Recharge", "📺 DTH Recharge");
        if (action == "📱 Mobile Recharge")
            await _navigationService.NavigateToAsync("MobilePage");
        else if (action == "📺 DTH Recharge")
            await _navigationService.NavigateToAsync("DTHPage");
    }

    [RelayCommand]
    private async Task EditReminderAsync(ReminderCardModel item)
    {
        if (item == null) return;

        bool confirm = await _dialogService.ShowConfirmationAsync("Confirm Edit", "Do you want to edit this record?");
        if (!confirm) return;

        var parameters = new Dictionary<string, object>
        {
            { "ReminderItem", item }
        };

        if (item.Category == "DTH")
            await _navigationService.NavigateToAsync("DTHPage", parameters);
        else
            await _navigationService.NavigateToAsync("MobilePage", parameters);
    }

    [RelayCommand]
    private async Task DeleteReminderAsync(ReminderCardModel item)
    {
        if (item == null) return;

        bool confirm = await _dialogService.ShowConfirmationAsync("Confirm Delete", "Are you sure you want to delete this reminder?");
        if (confirm)
        {
            await _notificationService.CancelReminderNotificationAsync(item.Id);
            await _reminderService.DeleteReminderAsync(item);
            AllReminders.Remove(item);
            await FilterRemindersAsync();
        }
    }

    [RelayCommand]
    private static async Task CallReminderAsync(ReminderCardModel item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.Number)) return;

        if (PhoneDialer.Default.IsSupported)
        {
            PhoneDialer.Default.Open(item.Number);
        }

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CopyReminderAsync(ReminderCardModel item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.Number)) return;

        await Clipboard.Default.SetTextAsync(item.Number);
        string message = item.Category == "DTH" ? "Consumer/Subscriber ID copied!" : "Mobile Number copied!";
        await _dialogService.ShowAlertAsync("Success", message);
    }

    [RelayCommand]
    private async Task SendWhatsAppReminderAsync(ReminderCardModel item)
    {
        if (item == null) return;
        await _notificationService.SendWhatsAppMessageAsync(item);
    }
}