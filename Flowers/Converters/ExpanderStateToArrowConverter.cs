using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Views;

namespace Flowers.Converters
{
    public class ExpanderStateToArrowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isExpanded)
            {
                return isExpanded ? "arrow_down.png" : "arrow_right.png"; // замените на ваши изображения стрелок
            }
            return "arrow_right.png"; // значение по умолчанию
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
