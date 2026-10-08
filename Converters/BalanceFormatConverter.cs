namespace ReminderProX.Converters;

public class BalanceFormatConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null) return "₹0";

        try
        {
            decimal balance = System.Convert.ToDecimal(value, culture);
            return balance switch
            {
                > 0 => $"+₹{balance:N0}",
                < 0 => $"-₹{Math.Abs(balance):N0}",
                _ => "₹0"
            };
        }
        catch
        {
            return "₹0";
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}