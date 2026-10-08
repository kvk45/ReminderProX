namespace ReminderProX.Converters;

public class AuthButtonColorConverter : IValueConverter
{
    private static readonly Color SignOutColor = Color.FromArgb("#D32F2F");
    private static readonly Color SignInColor = Color.FromArgb("#2196F3");

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool isSignedIn && isSignedIn ? SignOutColor : SignInColor;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}