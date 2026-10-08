namespace ReminderProX;

public partial class AppShell : Shell
{
    private readonly IServiceProvider _serviceProvider;

    public AppShell(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        RegisterAppRoutes();
    }

    private static void RegisterAppRoutes()
    {
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(RemindersPage), typeof(RemindersPage));
        Routing.RegisterRoute(nameof(MobilePage), typeof(MobilePage));
        Routing.RegisterRoute(nameof(DTHPage), typeof(DTHPage));
        Routing.RegisterRoute(nameof(LedgerPage), typeof(LedgerPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(TransactionPage), typeof(TransactionPage));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (ThemeSwitch != null)
        {
            bool isDark = Preferences.Default.Get("AppTheme_DarkMode", Application.Current?.UserAppTheme == AppTheme.Dark);
            ThemeSwitch.IsToggled = isDark;
        }

        CheckAndExecuteDailyAutoSync();
    }

    private void OnThemeSwitchToggled(object? sender, ToggledEventArgs e)
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
            Preferences.Default.Set("AppTheme_DarkMode", e.Value);
        }
    }

    private void CheckAndExecuteDailyAutoSync()
    {
        bool isSignedIn = Preferences.Default.Get("IsSignedIn", false);
        bool isAutoBackupEnabled = Preferences.Default.Get("AutoBackupEnabled", true);

        if (!isSignedIn || !isAutoBackupEnabled) return;

        DateTime lastSync = Preferences.Default.Get("LastAutoBackupDate", DateTime.MinValue);
        if ((DateTime.Now - lastSync).TotalHours >= 24)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var settingsVm = _serviceProvider.GetRequiredService<SettingsViewModel>();
                    var reminderService = _serviceProvider.GetRequiredService<IReminderService>();

                    var localReminders = await reminderService.GetAllRemindersAsync();
                    if (localReminders == null || localReminders.Count == 0)
                    {
                        await settingsVm.RestoreDataSilentlyAsync();
                    }
                    else
                    {
                        await settingsVm.BackupDataSilentlyAsync();
                    }
                }
                catch
                {
                }
            });
        }
    }

    private async void OnHomeClicked(object? sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToAsync("//HomePage");
    }

    private async void OnRemindersClicked(object? sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToAsync(nameof(RemindersPage));
    }

    private async void OnMobileClicked(object? sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToAsync(nameof(MobilePage));
    }

    private async void OnDTHClicked(object? sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToAsync(nameof(DTHPage));
    }

    private async void OnLedgerClicked(object? sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToAsync("//LedgerPage");
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        FlyoutIsPresented = false;
        await GoToAsync("//SettingsPage");
    }
}