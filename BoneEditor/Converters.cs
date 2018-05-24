using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace BoneEditor
{
    public class CheckBoxConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            foreach (var value in values)
            {
                if (value == DependencyProperty.UnsetValue) return false;

                if(value is double)
                    if (Math.Abs((double) value - 1.00) > 1.00) return true;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ValueToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = value is double d ? d : 0;

            //SolidColorBrush brush = Application.Current.TryFindResource("PlusBrush") as SolidColorBrush;

            if (x < 1) return Application.Current.MainWindow.TryFindResource("MinusBrush") as LinearGradientBrush;
            if(x > 1) return Application.Current.MainWindow.TryFindResource("PlusBrush") as LinearGradientBrush;
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
