namespace ReminderProX.Converters;

public class AuthButtonTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool isSignedIn && isSignedIn ? "Sign Out" : "Sign In with Microsoft";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}