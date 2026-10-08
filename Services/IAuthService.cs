namespace ReminderProX.Services;

public interface IAuthService
{
    Task<bool> SignInAsync();
    Task SignOutAsync();
    Task<string?> GetAccessTokenAsync();
    bool IsSignedIn { get; }
    string UserDisplayName { get; }
    string UserEmail { get; }
}