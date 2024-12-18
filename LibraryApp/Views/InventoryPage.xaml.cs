namespace LibraryApp.Views
{
    public partial class InventoryPage : ContentPage
    {
        private InventoryViewModel _inventoryViewModel;
        public InventoryPage()
        {
            InitializeComponent();
            _inventoryViewModel = new InventoryViewModel(new InventoryService(), new BookService());
            BindingContext = _inventoryViewModel;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is InventoryViewModel viewModel)
            {
                viewModel.OnDisappearing();
            }
        }
    }
}
