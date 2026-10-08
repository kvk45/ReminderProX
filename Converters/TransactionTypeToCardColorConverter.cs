namespace ReminderProX.Converters;

public class TransactionTypeToCardColorConverter : IValueConverter
{
    private static readonly Color DebitCardColor = Color.FromArgb("#F44336");
    private static readonly Color CreditCardColor = Color.FromArgb("#4CAF50");

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isDebit = value is string type && type.Equals("Debit", StringComparison.OrdinalIgnoreCase);
        return isDebit ? DebitCardColor : CreditCardColor;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}