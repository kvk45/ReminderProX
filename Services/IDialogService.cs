namespace ReminderProX.Services;

public interface IDialogService
{
    Task ShowAlertAsync(string title, string message, string cancel = "OK");
    Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No");
    Task<string?> ShowActionSheetAsync(string title, string cancel, string? destruction, params string[] buttons);
}