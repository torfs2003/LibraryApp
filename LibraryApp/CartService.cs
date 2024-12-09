namespace LibraryApp
{
    public static class CartService
    {
        private static ObservableCollection<Book> _cart = new ObservableCollection<Book>();

        public static ObservableCollection<Book> Cart
        {
            get => _cart;
            set => _cart = value;
        }
    }
}
