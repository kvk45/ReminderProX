namespace ReminderProX;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
#if ANDROID || IOS || MACCATALYST
            .UseLocalNotification()
#endif
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<DatabaseContext>();
        builder.Services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddSingleton<IReminderRepository, ReminderRepository>();
        builder.Services.AddSingleton<ILedgerRepository, LedgerRepository>();
        builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();

        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IOneDriveService, OneDriveService>();
        builder.Services.AddSingleton<IReminderService, ReminderService>();
        builder.Services.AddSingleton<ILedgerService, LedgerService>();
        builder.Services.AddSingleton<ITransactionService, TransactionService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IAppNotificationService, AppNotificationService>();
        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddTransient<BaseViewModel>();
        builder.Services.AddTransient<HomePageViewModel>();
        builder.Services.AddTransient<MobilePageViewModel>();
        builder.Services.AddTransient<DTHPageViewModel>();
        builder.Services.AddTransient<RemindersViewModel>();
        builder.Services.AddTransient<LedgerViewModel>();
        builder.Services.AddTransient<TransactionViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<MobilePage>();
        builder.Services.AddTransient<DTHPage>();
        builder.Services.AddTransient<RemindersPage>();
        builder.Services.AddTransient<LedgerPage>();
        builder.Services.AddTransient<TransactionPage>();
        builder.Services.AddTransient<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}