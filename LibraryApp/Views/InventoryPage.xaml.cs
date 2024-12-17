namespace LibraryApp.Views
{
    public partial class InventoryPage : ContentPage
    {
        private readonly InventoryViewModel _viewModel;

        public InventoryPage()
        {
            InitializeComponent();
            BindingContext = new InventoryViewModel(new InventoryService(), new BookService());
        }
    }
}
