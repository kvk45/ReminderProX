namespace ReminderProX.Services;

public interface IReminderService
{
    Task<List<ReminderCardModel>> GetAllRemindersAsync();
    Task<List<ReminderCardModel>> GetPagedRemindersAsync(int skip, int take);
    Task<List<ReminderCardModel>> SearchRemindersAsync(string query);
    Task<ReminderCardModel?> GetReminderByIdAsync(int id);
    Task<int> SaveReminderAsync(ReminderCardModel reminder);
    Task<int> DeleteReminderAsync(ReminderCardModel reminder);
    Task<int> DeleteReminderByIdAsync(int id);
    Task<int> ClearAllRemindersAsync();
}