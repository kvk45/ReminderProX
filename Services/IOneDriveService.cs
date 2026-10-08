namespace ReminderProX.Services;

public interface IOneDriveService
{
    Task<bool> UploadBackupAsync(string jsonData);
    Task<string?> DownloadBackupAsync();
}