namespace ReminderProX.Converters;

public class TransactionTypeToAlignmentConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isDebit = value is string type && type.Equals("Debit", StringComparison.OrdinalIgnoreCase);
        return isDebit ? LayoutOptions.Start : LayoutOptions.End;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}