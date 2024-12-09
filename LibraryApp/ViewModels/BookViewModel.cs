using System.Security.Cryptography.X509Certificates;

namespace LibraryApp.ViewModels
{
    public partial class BookViewModel : BaseViewModel
    {
        LibraryService libraryService;
        public ObservableCollection<Book> Books { get; } = new();



        private Book? _selectedBook;
        public Book? SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged();
            }
        }

        public ICommand GetBooksCommand { get; }
        public ICommand NavigateToMainPageCommand { get; }
        public ICommand NavigateToCartPageCommand { get; }
        public ICommand SaveBookCommand { get; }
        public ICommand ToggleBookCommand { get; }

        //private readonly HttpClient _httpClient;
        //private const string ApiUrl = "http://localhost:5000/api/books"; // Your API URL

        public BookViewModel(LibraryService libraryService)
        {
            Title = "Library Books";
            this.libraryService = libraryService;

            Books = new ObservableCollection<Book>();
            //_httpClient = new HttpClient(); // Initialize HttpClient
            /*
            var book1 = new Book("The Great Gatsby", "F. Scott Fitzgerald", "Fiction", "gatsby.jpg");

            Books.Add(book1);

            Console.WriteLine(book1.Id);


            var book2 = new Book("1984", "George Orwell", "Dystopian", "nineteen_eightyfour.jpg");

            Books.Add(book2);


            Console.WriteLine(book2.Id);

            var book3 = new Book("To Kill a Mockingbird", "Harper Lee", "Fiction", "mockingbird.jpg");

            Books.Add(book3);

            Console.WriteLine(book3.Id);
            */

            GetBooksCommand = new Command(async () => await GetBooksAsync());
            SaveBookCommand = new Command(async () => await SaveBook());
            NavigateToMainPageCommand = new Command(async () => await NavigateToMainPage());
            NavigateToCartPageCommand = new Command(async () => await NavigateToCartPage());
            ToggleBookCommand = new Command<Book>(ToggleBookInCart);
            this.libraryService = libraryService;

            // Fetch books from the API on initialization
            //GetBooksFromApi();
        }

        // Method to fetch books from the API
        /*
        private async Task GetBooksFromApi()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(ApiUrl);
                

                Books.Clear();
                foreach (var book in booksFromApi)
                {
                    Books.Add(book);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching books: {ex.Message}");
            }
        }*/




        private async Task SaveBook()
        {
            if (CartService.Cart.Count > 0)
            {
                await NavigateToCartPage();
            }
            else
            {
                if (Application.Current != null && Application.Current.MainPage != null)
                {

                    await Application.Current.MainPage.DisplayAlert("Empty Cart", "Your cart is empty. Please add books before saving.", "OK");
                }
                else
                {
                    Console.WriteLine("Unable to display alert because the Application.Current or MainPage is null.");
                }
            }
        }

        public async Task GetBooksAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                var books = await libraryService.GetBooks();
                foreach (var book in books)
                     Books.Add(book); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Unable to get the books: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }



        private async Task NavigateToMainPage()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async Task NavigateToCartPage()
        {
            await Shell.Current.GoToAsync("//CartPage");
        }

        public bool IsSaveButtonVisible => CartService.Cart.Count > 0;

        private void ToggleBookInCart(Book book)
        {
            if (book != null)
            {
                if (book.IsInCart)
                {
                    CartService.Cart.Remove(book);
                    book.IsInCart = false; 
                }
                else
                {
                    CartService.Cart.Add(book);
                    book.IsInCart = true; 
                }

                OnPropertyChanged(nameof(Books));
            }
        }
    }
}
