namespace ReminderProX.ViewModels;

public partial class MobilePageViewModel : BaseViewModel, IQueryAttributable
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
    public partial string MobileNumber { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Amount { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Validity { get; set; } = string.Empty;

    public MobilePageViewModel(
        IReminderService reminderService,
        IDialogService dialogService,
        INavigationService navigationService,
        IAppNotificationService notificationService)
    {
        _reminderService = reminderService;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _notificationService = notificationService;
        Title = "Mobile Recharge";
        InitializeOperators();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("ReminderItem", out var itemObj) && itemObj is ReminderCardModel item)
        {
            ReminderItem = item;
            UserName = item.Name;
            MobileNumber = item.Number;
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
            new() { Name = "Airtel", Logo = "operator_airtel.png", IsSelected = false },
            new() { Name = "BSNL", Logo = "operator_bsnl.png", IsSelected = false },
            new() { Name = "Jio", Logo = "operator_jio.png", IsSelected = false },
            new() { Name = "VI", Logo = "operator_vi.png", IsSelected = false }
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
    private async Task PickContactAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.ContactsRead>();
            }

            if (status == PermissionStatus.Granted)
            {
                var contact = await Microsoft.Maui.ApplicationModel.Communication.Contacts.Default.PickContactAsync();
                if (contact != null)
                {
                    UserName = contact.DisplayName ?? string.Empty;
                    var phone = contact.Phones?.FirstOrDefault()?.PhoneNumber ?? string.Empty;
                    MobileNumber = phone.Replace(" ", "").Replace("-", "").Replace("+91", "");
                }
            }
            else
            {
                await _dialogService.ShowAlertAsync("Permission Denied", "Contact permission is required to select contacts.");
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync("Error", ex.Message);
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;

        if (SelectedOperator == null)
        {
            await _dialogService.ShowAlertAsync("Validation", "Please select a mobile operator.");
            return;
        }

        if (string.IsNullOrWhiteSpace(MobileNumber))
        {
            await _dialogService.ShowAlertAsync("Validation", "Please enter a mobile number.");
            return;
        }

        bool confirm = await _dialogService.ShowConfirmationAsync("Confirm Save", "Do you want to save changes?");
        if (!confirm) return;

        IsBusy = true;
        try
        {
            int parsedValidity = int.TryParse(Validity, out int validDays) ? validDays : 28;
            decimal parsedAmount = decimal.TryParse(Amount, out decimal amt) ? amt : 0m;

            var reminder = ReminderItem ?? new ReminderCardModel();
            reminder.Category = "Mobile";
            reminder.Name = string.IsNullOrWhiteSpace(UserName) ? SelectedOperator.Name : UserName;
            reminder.OperatorName = SelectedOperator.Name;
            reminder.Number = MobileNumber;
            reminder.Amount = parsedAmount;
            reminder.ValidityDays = parsedValidity;
            reminder.DueDate = DateTime.Now.AddDays(parsedValidity);
            reminder.IsActive = true;

            await _reminderService.SaveReminderAsync(reminder);
            await _notificationService.ScheduleReminderNotificationAsync(reminder);

            await _dialogService.ShowAlertAsync("Success", "Mobile recharge reminder saved successfully!");
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
        MobileNumber = string.Empty;
        Amount = string.Empty;
        Validity = string.Empty;
        SelectedOperator = null;
        ReminderItem = null;
        foreach (var o in Operators) o.IsSelected = false;
    }
}