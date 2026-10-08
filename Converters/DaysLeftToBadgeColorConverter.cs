using System.Globalization;

namespace ReminderProX.Converters;

public class DaysLeftToBadgeColorConverter : IValueConverter
{
    private static readonly Color GreenColor = Color.FromArgb("#2E7D32");
    private static readonly Color OrangeColor = Color.FromArgb("#F57C00");
    private static readonly Color RedColor = Color.FromArgb("#D32F2F");

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ReminderCardModel reminder)
        {
            var expiry = reminder.DueDate != default
                ? reminder.DueDate
                : reminder.RechargedAt.AddDays(reminder.ValidityDays > 0 ? reminder.ValidityDays : 28);

            int daysLeft = (expiry.Date - DateTime.Today).Days;

            if (daysLeft > 7)
                return GreenColor;
            if (daysLeft >= 3)
                return OrangeColor;

            return RedColor;
        }

        return GreenColor;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}