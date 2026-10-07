using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace TaskManager.App.Converters;

public class TimeSpanToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimeSpan ts)
        {
            if (ts.TotalHours >= 1)
            {
                return $"{(int)ts.TotalHours}h {ts.Minutes}m";
            }
            return $"{ts.Minutes}m {ts.Seconds}s";
        }
        return "0m";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
