namespace LibraryApp.ViewModels
{
    public partial class CartViewModel : BaseViewModel
    {
        private ObservableCollection<Book> _cart = new ObservableCollection<Book>();
        public ObservableCollection<Book> Cart
        {
            get => _cart;
            set => SetProperty(ref _cart, value);
        }

        public CartViewModel()
        {
            Title = "My Cart";
            Cart = new ObservableCollection<Book>(CartService.Cart);
        }
    }
}