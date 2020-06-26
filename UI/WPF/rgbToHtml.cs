using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ColorPicker
{
    class rgbToHtml : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return colorToHtml((int[])value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string colorToHtml(int[] rgb)
        {
            StringBuilder builder = new StringBuilder("#FF");
            for (int i = 0; i < 3; ++i)
            {
                string part = rgb[i].ToString("X");
                if (part.Length == 1) builder.Append('0');
                builder.Append(part);
            }

            return builder.ToString();
        }
    }
}
