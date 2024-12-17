namespace LibraryApp.Views
{
    public partial class MainPage : ContentPage
    {
        private BookViewModel _bookViewModel;

        public MainPage()
        {
            InitializeComponent();
            _bookViewModel = new BookViewModel(new BookService(), new InventoryService());
            BindingContext = _bookViewModel;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Ensure OnDisappearing from ViewModel is called
            if (BindingContext is BookViewModel viewModel)
            {
                viewModel.OnDisappearing();
            }
        }
    }
}