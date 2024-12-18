namespace LibraryApp.Views;

public partial class AdminPage : ContentPage
{
    private AdminViewModel _adminViewModel;
    public AdminPage()
	{
		InitializeComponent();
        _adminViewModel = new AdminViewModel(new BookService(), new InventoryService(), new UserService());
        BindingContext = _adminViewModel;
    }

}
