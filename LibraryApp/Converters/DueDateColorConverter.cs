using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApp.Converters
{
    public class DueDateColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dueDate)
            {
                var currentDate = DateTime.Now;
                var daysLeft = (dueDate - currentDate).Days;

                if (daysLeft > 0 && daysLeft < 10)
                {
                    return Colors.Orange;
                }
                else if (daysLeft >= 10)
                {
                    return Colors.Green;
                }
                else if (daysLeft == 0)
                {
                    return Colors.Red;
                }
                else
                {
                    return Colors.DarkRed;
                }
            }
            return Colors.Black;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
