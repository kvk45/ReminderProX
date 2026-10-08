namespace ReminderProX.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IAuthService _authService;
    private readonly IOneDriveService _oneDriveService;
    private readonly IReminderService _reminderService;
    private readonly ILedgerService _ledgerService;
    private readonly ITransactionService _transactionService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    public partial UserModel CurrentUser { get; set; }

    [ObservableProperty]
    public partial bool IsDarkMode { get; set; }

    [ObservableProperty]
    public partial bool IsAutoBackupEnabled { get; set; }

    public SettingsViewModel(
        IAuthService authService,
        IOneDriveService oneDriveService,
        IReminderService reminderService,
        ILedgerService ledgerService,
        ITransactionService transactionService,
        IDialogService dialogService)
    {
        _authService = authService;
        _oneDriveService = oneDriveService;
        _reminderService = reminderService;
        _ledgerService = ledgerService;
        _transactionService = transactionService;
        _dialogService = dialogService;
        Title = "Settings";

        CurrentUser = new UserModel();
        IsDarkMode = Preferences.Default.Get("AppTheme_DarkMode", Application.Current?.UserAppTheme == AppTheme.Dark);
        IsAutoBackupEnabled = Preferences.Default.Get("AutoBackupEnabled", true);

        bool signedIn = Preferences.Default.Get("IsSignedIn", false);
        if (signedIn)
        {
            CurrentUser.IsSignedIn = true;
            CurrentUser.DisplayName = Preferences.Default.Get("UserDisplayName", string.Empty);
            CurrentUser.Email = Preferences.Default.Get("UserEmail", string.Empty);
        }
    }

    partial void OnIsDarkModeChanged(bool value)
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
            Preferences.Default.Set("AppTheme_DarkMode", value);
        }
    }

    partial void OnIsAutoBackupEnabledChanged(bool value)
    {
        Preferences.Default.Set("AutoBackupEnabled", value);
    }

    [RelayCommand]
    private async Task ToggleAuthAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            if (CurrentUser.IsSignedIn)
            {
                await _authService.SignOutAsync();
                CurrentUser.IsSignedIn = false;
                CurrentUser.DisplayName = string.Empty;
                CurrentUser.Email = string.Empty;
            }
            else
            {
                bool success = await _authService.SignInAsync();
                if (success)
                {
                    CurrentUser.IsSignedIn = true;
                    CurrentUser.DisplayName = _authService.UserDisplayName;
                    CurrentUser.Email = _authService.UserEmail;

                    _ = Task.Run(async () =>
                    {
                        var reminders = await _reminderService.GetAllRemindersAsync();
                        if (reminders == null || reminders.Count == 0)
                        {
                            await RestoreDataSilentlyAsync();
                        }
                        else
                        {
                            await BackupDataSilentlyAsync();
                        }
                    });
                }
                else
                {
                    await _dialogService.ShowAlertAsync("Auth Failed", "Microsoft sign-in failed or was cancelled.");
                }
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
    private async Task BackupDataAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            bool success = await BackupDataSilentlyAsync();
            if (success)
            {
                await _dialogService.ShowAlertAsync("Success", "Data backed up to OneDrive successfully!");
            }
            else
            {
                await _dialogService.ShowAlertAsync("Failed", "Failed to upload backup to OneDrive. Please verify Microsoft account permissions.");
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
    private async Task RestoreDataAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            bool success = await RestoreDataSilentlyAsync();
            if (success)
            {
                await _dialogService.ShowAlertAsync("Success", "Data restored from OneDrive successfully!");
            }
            else
            {
                await _dialogService.ShowAlertAsync("Restore Failed", "No backup found or failed to restore.");
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

    public async Task<bool> BackupDataSilentlyAsync()
    {
        try
        {
            var reminders = await _reminderService.GetAllRemindersAsync();
            var ledgers = await _ledgerService.GetAllLedgersAsync();
            var transactions = await _transactionService.GetAllTransactionsAsync();

            var backupObj = new BackupDataModel
            {
                BackupDate = DateTime.Now,
                Reminders = reminders ?? [],
                Ledgers = ledgers ?? [],
                Transactions = transactions ?? []
            };

            string json = JsonSerializer.Serialize(backupObj, JsonOptions);
            bool success = await _oneDriveService.UploadBackupAsync(json);
            if (success)
            {
                Preferences.Default.Set("LastAutoBackupDate", DateTime.Now);
            }
            return success;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RestoreDataSilentlyAsync()
    {
        try
        {
            string? json = await _oneDriveService.DownloadBackupAsync();
            if (string.IsNullOrEmpty(json)) return false;

            var backupObj = JsonSerializer.Deserialize<BackupDataModel>(json, JsonOptions);
            if (backupObj == null) return false;

            if (backupObj.Reminders != null && backupObj.Reminders.Count > 0)
            {
                await _reminderService.ClearAllRemindersAsync();
                foreach (var rem in backupObj.Reminders)
                {
                    rem.Id = 0;
                    await _reminderService.SaveReminderAsync(rem);
                }
            }

            if (backupObj.Ledgers != null && backupObj.Ledgers.Count > 0)
            {
                await _ledgerService.ClearAllLedgersAsync();
                if (backupObj.Transactions != null)
                {
                    await _transactionService.ClearAllTransactionsAsync();
                }

                foreach (var led in backupObj.Ledgers)
                {
                    int oldLedgerId = led.Id;
                    led.Id = 0;
                    await _ledgerService.SaveLedgerAsync(led);
                    int newLedgerId = led.Id;

                    if (backupObj.Transactions != null)
                    {
                        var relatedTxns = backupObj.Transactions.Where(t => t.LedgerId == oldLedgerId).ToList();
                        foreach (var txn in relatedTxns)
                        {
                            txn.Id = 0;
                            txn.LedgerId = newLedgerId;
                            await _transactionService.SaveTransactionAsync(txn);
                        }
                    }
                }
            }

            WeakReferenceMessenger.Default.Send(new DataRestoredMessage());
            return true;
        }
        catch
        {
            return false;
        }
    }
}