namespace LibraryApp.Converters
{
    public class BoolToAddRemoveButtonTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isInCart)
            {
                return isInCart ? "Remove from Cart" : "Add to Cart";
            }
            return "Add to Cart"; 
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is string text && text.Equals("Add to Cart", StringComparison.OrdinalIgnoreCase);
        }
    }
}
