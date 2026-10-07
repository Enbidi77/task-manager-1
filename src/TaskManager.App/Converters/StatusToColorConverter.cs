using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TaskManager.Core.Enums;

namespace TaskManager.App.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TaskItemStatus status)
        {
            return status switch
            {
                TaskItemStatus.Inbox => Color.FromArgb("#797775"),
                TaskItemStatus.Todo => Color.FromArgb("#0078D4"),
                TaskItemStatus.InProgress => Color.FromArgb("#FF8C00"),
                TaskItemStatus.Blocked => Color.FromArgb("#D13438"),
                TaskItemStatus.Review => Color.FromArgb("#8E56FF"),
                TaskItemStatus.Done => Color.FromArgb("#107C10"),
                TaskItemStatus.Cancelled => Color.FromArgb("#A19F9D"),
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
