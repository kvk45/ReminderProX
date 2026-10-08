namespace ReminderProX.Converters;

public class OperatorSelectedToBorderConverter : IValueConverter
{
    private static readonly Color SelectedBorderColor = Color.FromArgb("#2196F3");
    private static readonly Color DefaultBorderColor = Colors.Transparent;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool isSelected && isSelected ? SelectedBorderColor : DefaultBorderColor;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}