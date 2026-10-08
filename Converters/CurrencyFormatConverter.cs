namespace ReminderProX.Converters;

public class CurrencyFormatConverter : IValueConverter
{
    private static readonly CultureInfo IndianCulture = new("en-IN");

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null) return "₹0";

        try
        {
            decimal amount = System.Convert.ToDecimal(value, culture);
            return amount.ToString("C0", IndianCulture);
        }
        catch
        {
            return "₹0";
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}