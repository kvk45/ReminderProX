namespace ReminderProX.Converters;

public class DaysLeftToProgressConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ReminderCardModel reminder)
        {
            int totalDays = reminder.ValidityDays > 0
                ? reminder.ValidityDays
                : (reminder.DueDate.Date - reminder.RechargedAt.Date).Days;

            if (totalDays <= 0) totalDays = 1;

            int daysLeft = (reminder.DueDate.Date - DateTime.Today).Days;
            if (daysLeft < 0) daysLeft = 0;

            // எஞ்சிய நாட்கள் / மொத்த வேலிடிட்டி நாட்கள் (1.0 லிருந்து 0.0 வரை குறையும்)
            double ratio = (double)daysLeft / totalDays;
            return Math.Clamp(ratio, 0.0, 1.0);
        }
        return 0.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}