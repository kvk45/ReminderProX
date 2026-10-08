namespace ReminderProX.Data;

public interface IReminderRepository : IRepository<ReminderCardModel>
{
    Task<List<ReminderCardModel>> GetActiveRemindersSortedAsync();
    Task<List<ReminderCardModel>> GetPagedActiveRemindersSortedAsync(int skip, int take);
    Task<List<ReminderCardModel>> SearchRemindersAsync(string query);
    Task<int> ClearAllRemindersAsync();
}