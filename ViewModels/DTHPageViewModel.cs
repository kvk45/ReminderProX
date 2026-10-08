namespace ReminderProX.ViewModels;

public partial class DTHPageViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IReminderService _reminderService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly IAppNotificationService _notificationService;

    [ObservableProperty]
    public partial ReminderCardModel? ReminderItem { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<OperatorOption> Operators { get; set; } = [];

    [ObservableProperty]
    public partial OperatorOption? SelectedOperator { get; set; }

    [ObservableProperty]
    public partial string UserName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ConsumerNumber { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Amount { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Validity { get; set; } = string.Empty;

    public DTHPageViewModel(
        IReminderService reminderService,
        IDialogService dialogService,
        INavigationService navigationService,
        IAppNotificationService notificationService)
    {
        _reminderService = reminderService;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _notificationService = notificationService;
        Title = "DTH Recharge";
        InitializeOperators();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("ReminderItem", out var itemObj) && itemObj is ReminderCardModel item)
        {
            ReminderItem = item;
            UserName = item.Name;
            ConsumerNumber = item.Number;
            Amount = item.Amount > 0 ? item.Amount.ToString() : string.Empty;
            Validity = item.ValidityDays.ToString();

            var matchingOp = Operators.FirstOrDefault(o => o.Name.Equals(item.OperatorName, StringComparison.OrdinalIgnoreCase));
            if (matchingOp != null)
            {
                SelectOperator(matchingOp);
            }
        }
    }

    private void InitializeOperators()
    {
        Operators =
        [
            new() { Name = "Airtel Digital TV", Logo = "dth_airtel.png", IsSelected = false },
            new() { Name = "Dish TV", Logo = "dth_dishtv.png", IsSelected = false },
            new() { Name = "Sun Direct", Logo = "dth_sundirect.png", IsSelected = false },
            new() { Name = "Tata Play", Logo = "dth_tataplay.png", IsSelected = false }
        ];
    }

    [RelayCommand]
    private void SelectOperator(OperatorOption op)
    {
        if (SelectedOperator == op)
        {
            SelectedOperator = null;
            foreach (var o in Operators) o.IsSelected = false;
        }
        else
        {
            SelectedOperator = op;
            foreach (var o in Operators) o.IsSelected = (o == op);
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;

        if (SelectedOperator == null)
        {
            await _dialogService.ShowAlertAsync("Validation", "Please select a DTH operator.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ConsumerNumber))
        {
            await _dialogService.ShowAlertAsync("Validation", "Please enter a Subscriber/Consumer ID.");
            return;
        }

        bool confirm = await _dialogService.ShowConfirmationAsync("Confirm Save", "Do you want to save changes?");
        if (!confirm) return;

        IsBusy = true;
        try
        {
            int parsedValidity = int.TryParse(Validity, out int validDays) ? validDays : 30;
            decimal parsedAmount = decimal.TryParse(Amount, out decimal amt) ? amt : 0m;

            var reminder = ReminderItem ?? new ReminderCardModel();
            reminder.Category = "DTH";
            reminder.Name = string.IsNullOrWhiteSpace(UserName) ? SelectedOperator.Name : UserName;
            reminder.OperatorName = SelectedOperator.Name;
            reminder.Number = ConsumerNumber;
            reminder.Amount = parsedAmount;
            reminder.ValidityDays = parsedValidity;
            reminder.DueDate = DateTime.Now.AddDays(parsedValidity);
            reminder.IsActive = true;

            await _reminderService.SaveReminderAsync(reminder);
            await _notificationService.ScheduleReminderNotificationAsync(reminder);

            await _dialogService.ShowAlertAsync("Success", "DTH recharge reminder saved successfully!");
            ClearForm();
            await _navigationService.GoBackAsync();
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
    private void Clear() => ClearForm();

    private void ClearForm()
    {
        UserName = string.Empty;
        ConsumerNumber = string.Empty;
        Amount = string.Empty;
        Validity = string.Empty;
        SelectedOperator = null;
        ReminderItem = null;
        foreach (var o in Operators) o.IsSelected = false;
    }
}