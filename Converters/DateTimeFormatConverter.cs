namespace ReminderProX.Converters;

public class DateTimeFormatConverter : IValueConverter
{
    private const string DateFormat = "dd MMM yyyy, hh:mm tt";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToString(DateFormat, CultureInfo.InvariantCulture);
        }

        if (value is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString(DateFormat, CultureInfo.InvariantCulture);
        }

        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}