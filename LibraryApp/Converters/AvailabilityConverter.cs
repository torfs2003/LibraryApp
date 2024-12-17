namespace LibraryApp.Converters
{
    public class AvailabilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool isAvailable = (bool)value;
            return isAvailable ? "Available" : "Out of stock";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}