using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TaskManager.Core.Enums;

namespace TaskManager.App.Converters;

public class PriorityToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TaskPriority priority)
        {
            return priority switch
            {
                TaskPriority.Urgent => Color.FromArgb("#C50F1F"),
                TaskPriority.High => Color.FromArgb("#D13438"),
                TaskPriority.Medium => Color.FromArgb("#FFB900"),
                TaskPriority.Low => Color.FromArgb("#107C10"),
                _ => Color.FromArgb("#797775")
            };
        }
        return Color.FromArgb("#797775");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
