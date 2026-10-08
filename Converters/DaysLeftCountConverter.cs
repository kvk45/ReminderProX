namespace ReminderProX.Converters;

public class DaysLeftCountConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ReminderCardModel reminder)
        {
            var expiry = reminder.DueDate != default
                ? reminder.DueDate
                : reminder.RechargedAt.AddDays(reminder.ValidityDays > 0 ? reminder.ValidityDays : 28);

            int diff = (expiry.Date - DateTime.Today).Days;
            return diff < 0 ? 0 : diff;
        }
        return 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}