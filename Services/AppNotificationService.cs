namespace ReminderProX.Services;

public class AppNotificationService(IDialogService dialogService) : IAppNotificationService
{
    public async Task ScheduleReminderNotificationAsync(ReminderCardModel reminder)
    {
        if (reminder == null || !reminder.IsActive) return;

        try
        {
#if ANDROID || IOS || MACCATALYST
            DateTime notifyDate = reminder.DueDate.Date.AddDays(-1).AddHours(9);

            if (notifyDate <= DateTime.Now)
            {
                notifyDate = DateTime.Now.AddSeconds(5);
            }

            var request = new NotificationRequest
            {
                NotificationId = reminder.Id,
                Title = "Recharge Expiring Tomorrow!",
                Description = $"{reminder.Name} ({reminder.OperatorName} - {reminder.Number}) expires tomorrow. Amount: ₹{reminder.Amount:N0}",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyDate
                }
            };

            await LocalNotificationCenter.Current.Show(request);
#else
            await Task.CompletedTask;
#endif
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Local Notification Error: {ex.Message}");
        }
    }

    public Task CancelReminderNotificationAsync(int reminderId)
    {
        try
        {
#if ANDROID || IOS || MACCATALYST
            LocalNotificationCenter.Current.Cancel(reminderId);
#endif
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Cancel Notification Error: {ex.Message}");
        }
        return Task.CompletedTask;
    }

    public async Task CheckAndScheduleAllAsync(List<ReminderCardModel> reminders)
    {
        if (reminders == null || reminders.Count == 0) return;

        foreach (var rem in reminders.Where(r => r.IsActive))
        {
            await ScheduleReminderNotificationAsync(rem);
        }
    }

    public async Task SendWhatsAppMessageAsync(ReminderCardModel reminder)
    {
        if (reminder == null || string.IsNullOrWhiteSpace(reminder.Number)) return;

        try
        {
            string cleanNumber = reminder.Number.Replace(" ", "").Replace("-", "").Replace("+", "");
            if (cleanNumber.Length == 10)
            {
                cleanNumber = "91" + cleanNumber;
            }

            string textMessage = Uri.EscapeDataString($"Hello {reminder.Name}, your {reminder.OperatorName} recharge ({reminder.Number}) expires tomorrow on {reminder.DueDate:dd MMM yyyy}. Please recharge soon.");
            string whatsappUrl = $"https://wa.me/{cleanNumber}?text={textMessage}";

            var uri = new Uri(whatsappUrl);
            if (await Launcher.Default.CanOpenAsync(uri))
            {
                await Launcher.Default.OpenAsync(uri);
            }
            else
            {
                await dialogService.ShowAlertAsync("Error", "WhatsApp is not installed on this device.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"WhatsApp Launch Error: {ex.Message}");
            await dialogService.ShowAlertAsync("Error", "Could not open WhatsApp.");
        }
    }
}