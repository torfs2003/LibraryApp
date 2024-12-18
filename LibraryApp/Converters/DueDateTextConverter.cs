namespace LibraryApp.Converters
{
    public class DueDateTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dueDate)
            {
                var currentDate = DateTime.Now;
                var daysLeft = (dueDate - currentDate).Days;

                if (daysLeft > 0)
                {
                    return $"Due Date: {dueDate:MM/dd/yyyy} \n({daysLeft} day{(daysLeft > 1 ? "s" : "")} left)";
                }
                else if (daysLeft == 0)
                {
                    return $"Due Date: Today";
                }
                else
                {
                    return $"Due Date: {dueDate:MM/dd/yyyy} \n(Overdue by {-daysLeft} day{(-daysLeft > 1 ? "s" : "")})";
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
