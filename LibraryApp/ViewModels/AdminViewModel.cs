namespace LibraryApp.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private readonly InventoryService _inventoryService;
        private readonly UserService _userService;
        private readonly BookService _bookService;

        public ObservableCollection<object> Items { get; set; } = new ObservableCollection<object>();

        public ICommand LogoutCommand { get; }


        public AdminViewModel()
        {
            Title = "Admin Page";
            _inventoryService = new InventoryService();
            _userService = new UserService();
            _bookService = new BookService();

            LogoutCommand = new Command(Logout);
        }


        private async void Logout()
        {

            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}