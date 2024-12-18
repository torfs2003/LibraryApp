namespace LibraryApp.ViewModels
{
    public partial class BookViewModel : BaseViewModel
    {
        readonly BookService _bookService;
        readonly InventoryService _inventoryService;

        public ObservableCollection<Book> Books { get; } = new();
        public ObservableCollection<Book> Cart { get; } = new();



        private bool _isSaveButtonVisible;
        public bool IsSaveButtonVisible
        {
            get => _isSaveButtonVisible || Cart.Count > 0;
            set
            {
                if (_isSaveButtonVisible == value) return;
                _isSaveButtonVisible = value;
                OnPropertyChanged(nameof(IsSaveButtonVisible));
            }
        }

        public static class Notifier
        {
            public static event Action InventoryUpdated;

            public static void NotifyInventoryUpdated()
            {
                InventoryUpdated?.Invoke();
            }
        }


        public ICommand LogoutCommand { get; }
        public ICommand SaveBookCommand { get; }
        public ICommand ToggleBookCommand { get; }

        // Constructor
        public BookViewModel(BookService bookService, InventoryService inventoryService)
        {
            Title = "Library Books";
            _bookService = bookService;
            _inventoryService = inventoryService;


            // Commands for various actions
            LogoutCommand = new Command(Logout);
            SaveBookCommand = new Command(async () => await SaveBooks());
            ToggleBookCommand = new Command<Book>(ToggleBook);

            Notifier.InventoryUpdated += OnInventoryUpdated;

            Task.Run(async () => await GetBooksAsync());
        }

        private async void OnInventoryUpdated()
        {
            try
            {
                // Fetch updated book data from the database
                var updatedBooks = await _bookService.GetAllBooks();

                foreach (var updatedBook in updatedBooks)
                {
                    var existingBook = Books.FirstOrDefault(b => b.Id == updatedBook.Id);
                    if (existingBook != null)
                    {
                       
                        existingBook.Stock = updatedBook.Stock;
                        OnPropertyChanged(nameof(Books));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating book stock in UI: {ex.Message}");
            }
        }

        private async void Logout()
        {
            Cart.Clear();
            Books.Clear();

            await Shell.Current.GoToAsync("//LoginPage");
        }



        // Fetch the books that the user currently owns in their inventory
        public async Task GetBooksAsync()
        {
            if (IsBusy)
                return;

            if (Books.Count > 0)
                return;

            try
            {
                IsBusy = true;

                // Fetch all books using the BookService
                var allBooks = await _bookService.GetAllBooks();

                Books.Clear();

                // Add each book to the Books collection
                foreach (var bookDetails in allBooks)
                {
                    var book = new Book(
                        bookDetails.Id,
                        bookDetails.Title,
                        bookDetails.Author,
                        bookDetails.Genre,
                        bookDetails.Image,
                        bookDetails.Stock
                    );
                    Books.Add(book);
                }

                Console.WriteLine($"Fetched {Books.Count} books from the server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching books: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", $"Unable to get the books: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        

        private async void ToggleBook(Book book)
        {
            try
            {
               
                if (Cart.Contains(book))
                {
                    Cart.Remove(book); 
                    book.IsAddButtonVisibleBook = true; 
                    book.IsRemoveButtonVisibleBook = false;
                }
                else
                {
                    
                    Cart.Add(book);
                    book.IsAddButtonVisibleBook = false;
                    book.IsRemoveButtonVisibleBook = true; 
                }

                OnPropertyChanged(nameof(Cart));
                OnPropertyChanged(nameof(IsSaveButtonVisible));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }



        // Save books in the User Inventory
        private async Task SaveBooks()
        {
            if (Cart.Count == 0)
                return;

            bool success = true;

            foreach (var book in Cart)
            {
                var inventoryItem = new InventoryWriteDto
                {
                    BookId = book.Id,
                    UserId = UserId,
                    DueDate = DateTime.Now.AddMonths(1)
                };

                bool bookSuccess = await _inventoryService.BorrowBooks(inventoryItem);
                if (!bookSuccess)
                {
                    success = false;
                    break;
                }
                var bookDetails = await _bookService.GetBookById(book.Id);
                if (bookDetails != null)
                {
                    var updatedBook = new BookUpdateDto
                    {
                        Title = bookDetails.Title,
                        Author = bookDetails.Author,
                        Genre = bookDetails.Genre,
                        Image = bookDetails.Image,
                        Stock = bookDetails.Stock - 1
                    };
                    bool updateSuccess = await _bookService.UpdateBook(book.Id, updatedBook);
                    if (updateSuccess)
                    {
                        var bookInCollection = Books.FirstOrDefault(b => b.Id == book.Id);
                        if (bookInCollection != null)
                        {
                            bookInCollection.Stock = updatedBook.Stock;
                            bookInCollection.IsAddButtonVisibleBook = true;
                            bookInCollection.IsRemoveButtonVisibleBook = false;
                        }

                        Notifier.NotifyInventoryUpdated();
                    }
                    else
                    {
                        success = false;
                        break;
                    }
                }
                else
                {
                    success = false;
                    break;
                }
            }

            if (success)
            {
                // Clear the cart after all books have been successfully borrowed
                Cart.Clear();

                // Explicitly notify the UI that the cart is empty and the "Save" button should be disabled
                OnPropertyChanged(nameof(Cart)); // This will notify the UI about the cart change
                OnPropertyChanged(nameof(IsSaveButtonVisible)); // This will update the button visibility

                // Show confirmation modal
                bool navigateToInventory = await Shell.Current.DisplayAlert(
                    "Success",
                    "Books borrowed and stock updated successfully.\nDo you want to go to your inventory?",
                    "Yes",
                    "No"
                );

                if (navigateToInventory)
                {
                    await Shell.Current.GoToAsync("//InventoryPage");

                }
            }
            else
            {
                // Display error message if any part of the process failed
                await Shell.Current.DisplayAlert("Error", "There was an issue updating the book stock.", "OK");
            }
        }

        

        public void OnDisappearing()
        {
            // Reset the buttons for each book
            foreach (var book in Cart)
            {
                book.IsAddButtonVisibleBook = true;
                book.IsRemoveButtonVisibleBook = false;
            }
            Cart.Clear();
            OnPropertyChanged(nameof(IsSaveButtonVisible));
        }

        ~BookViewModel()
        {
            Notifier.InventoryUpdated -= OnInventoryUpdated;
        }

    }
}