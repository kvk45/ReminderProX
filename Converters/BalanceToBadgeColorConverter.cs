namespace ReminderProX.Converters;

public class BalanceToBadgeColorConverter : IValueConverter
{
    private static readonly Color PositiveColor = Color.FromArgb("#4CAF50");
    private static readonly Color NegativeColor = Color.FromArgb("#F44336");
    private static readonly Color NeutralColor = Color.FromArgb("#757575");

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null) return NeutralColor;

        try
        {
            decimal balance = System.Convert.ToDecimal(value, culture);
            return balance switch
            {
                > 0 => PositiveColor,
                < 0 => NegativeColor,
                _ => NeutralColor
            };
        }
        catch
        {
            return NeutralColor;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}