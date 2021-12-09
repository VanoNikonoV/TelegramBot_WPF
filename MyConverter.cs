using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TelegramBot_WPF
{    
    /// <summary>
    /// Конвектор для даты
    /// </summary>
    public class MyConverter : IValueConverter
        {
            public object Convert(object o, Type type,
                object parameter, CultureInfo culture)
            {
                var date = (DateTime)o;
                switch (type.Name)
                {
                    case "String":

                        return date.ToShortTimeString();
                    case "Brush":
                        return Brushes.Red;
                    default:
                        return o;
                }
            }

            public object ConvertBack(object o, Type type,
                object parameter, CultureInfo culture) => null;
        }
}

