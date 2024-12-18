namespace LibraryApp.Views
{
    public partial class InventoryPage : ContentPage
    {
        public InventoryPage()
        {
            InitializeComponent();
            BindingContext = new InventoryViewModel(new InventoryService(), new BookService());
        }
    }
}
