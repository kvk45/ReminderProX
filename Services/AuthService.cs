namespace ReminderProX.Services;

public class AuthService : IAuthService
{
    private const string ClientId = "541dd339-b227-4186-bf92-38d77bd61369";
    private const string Authority = "https://login.microsoftonline.com/common";
    private static readonly string[] Scopes = ["User.Read", "Files.ReadWrite", "Files.ReadWrite.AppFolder", "offline_access"];

    private readonly IPublicClientApplication _pca;
    private AuthenticationResult? _authResult;

    public bool IsSignedIn => _authResult is not null;
    public string UserDisplayName => _authResult?.Account?.Username ?? Preferences.Default.Get("UserDisplayName", string.Empty);
    public string UserEmail => _authResult?.Account?.Username ?? Preferences.Default.Get("UserEmail", string.Empty);

    public AuthService()
    {
        var builder = PublicClientApplicationBuilder.Create(ClientId)
            .WithAuthority(Authority);

#if WINDOWS
        builder = builder.WithRedirectUri("http://localhost:5000");
#else
        builder = builder.WithRedirectUri($"msal{ClientId}://auth");
#endif

        _pca = builder.Build();
    }

    public async Task<bool> SignInAsync()
    {
        try
        {
            var accounts = new List<IAccount>(await _pca.GetAccountsAsync());
            var firstAccount = accounts.Count > 0 ? accounts[0] : null;

            if (firstAccount != null)
            {
                _authResult = await _pca.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync();
            }
            else
            {
                _authResult = await ConfigureInteractiveTokenBuilder(_pca.AcquireTokenInteractive(Scopes)).ExecuteAsync();
            }

            if (IsSignedIn)
            {
                Preferences.Default.Set("IsSignedIn", true);
                Preferences.Default.Set("UserDisplayName", UserDisplayName);
                Preferences.Default.Set("UserEmail", UserEmail);
            }

            return IsSignedIn;
        }
        catch (MsalUiRequiredException)
        {
            try
            {
                _authResult = await ConfigureInteractiveTokenBuilder(_pca.AcquireTokenInteractive(Scopes)).ExecuteAsync();
                if (IsSignedIn)
                {
                    Preferences.Default.Set("IsSignedIn", true);
                    Preferences.Default.Set("UserDisplayName", UserDisplayName);
                    Preferences.Default.Set("UserEmail", UserEmail);
                }
                return IsSignedIn;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Auth Exception: {ex.Message}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Sign In Error: {ex.Message}");
            return false;
        }
    }

    private static AcquireTokenInteractiveParameterBuilder ConfigureInteractiveTokenBuilder(AcquireTokenInteractiveParameterBuilder builder)
    {
#if ANDROID
        if (Microsoft.Maui.ApplicationModel.Platform.CurrentActivity is { } activity)
        {
            return builder.WithParentActivityOrWindow(activity);
        }
#elif WINDOWS
        var windows = Application.Current?.Windows;
        var mainWindow = windows != null && windows.Count > 0 ? windows[0] : null;
        var window = mainWindow?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        if (window != null)
        {
            var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            return builder.WithParentActivityOrWindow(handle);
        }
#endif
        return builder;
    }

    public async Task SignOutAsync()
    {
        var accounts = await _pca.GetAccountsAsync();
        foreach (var account in accounts)
        {
            await _pca.RemoveAsync(account);
        }
        _authResult = null;
        Preferences.Default.Remove("IsSignedIn");
        Preferences.Default.Remove("UserDisplayName");
        Preferences.Default.Remove("UserEmail");
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        if (_authResult is not null && _authResult.ExpiresOn > DateTimeOffset.UtcNow)
        {
            return _authResult.AccessToken;
        }

        try
        {
            var accounts = new List<IAccount>(await _pca.GetAccountsAsync());
            var account = accounts.Count > 0 ? accounts[0] : null;

            if (account != null)
            {
                _authResult = await _pca.AcquireTokenSilent(Scopes, account).ExecuteAsync();
                return _authResult?.AccessToken;
            }
            else
            {
                _authResult = await ConfigureInteractiveTokenBuilder(_pca.AcquireTokenInteractive(Scopes)).ExecuteAsync();
                return _authResult?.AccessToken;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"GetAccessToken Error: {ex.Message}");
        }

        return _authResult?.AccessToken;
    }
}