namespace ReminderProX.Services;

public interface IAppNotificationService
{
    Task ScheduleReminderNotificationAsync(ReminderCardModel reminder);
    Task CancelReminderNotificationAsync(int reminderId);
    Task CheckAndScheduleAllAsync(List<ReminderCardModel> reminders);
    Task SendWhatsAppMessageAsync(ReminderCardModel reminder);
}