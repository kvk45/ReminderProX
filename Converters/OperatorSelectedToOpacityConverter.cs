namespace ReminderProX.Converters;

public class OperatorSelectedToOpacityConverter : IValueConverter
{
    private const double SelectedOpacity = 1.0;
    private const double DefaultOpacity = 0.4;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool isSelected && isSelected ? SelectedOpacity : DefaultOpacity;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}