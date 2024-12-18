using static LibraryApp.ViewModels.BookViewModel;

namespace LibraryApp.ViewModels
{
    public partial class InventoryViewModel : BaseViewModel
    {
        readonly InventoryService _inventoryService;
        readonly BookService _bookService;

        public ObservableCollection<Book> Cart { get; set; } = new();

        public ObservableCollection<Inventory> Inventory { get; set; } = new();

        

        private bool _isReturnButtonVisible;
        public bool IsReturnButtonVisible
        {
            get => _isReturnButtonVisible || Cart.Count > 0;
            set
            {
                if (_isReturnButtonVisible == value) return;
                _isReturnButtonVisible = value;
                OnPropertyChanged(nameof(IsReturnButtonVisible));
            }
        }

        public ICommand LogoutCommand { get; }
        public ICommand ReturnBookCommand { get; }
        public ICommand ToggleInventoryCommand { get; }

      
        public InventoryViewModel(InventoryService inventoryService, BookService bookService)
        {
            Title = "My Inventory";
            _inventoryService = inventoryService;
            _bookService = bookService;

            LogoutCommand = new Command(Logout);
            ToggleInventoryCommand = new Command<Book>(async (book) => await ToggleInventory(book));
            ReturnBookCommand = new Command(async () => await ReturnBooks());

            BookViewModel.Notifier.InventoryUpdated += OnInventoryUpdated;

            Task.Run(async () => await GetInventoryAsync(UserId));
        }

        private void OnInventoryUpdated()
        {
            // Re-fetch inventory data
            Task.Run(async () => await GetInventoryAsync(UserId));
        }

        private async void Logout()
        {
            Preferences.Remove("userEmail");
            Preferences.Remove("userPassword");

            Preferences.Remove("userToken");

            await Shell.Current.GoToAsync("//LoginPage");
        }



        public async Task GetInventoryAsync(int userId)
        {
            try
            {
                IsBusy = true;

                var userInventory = await _inventoryService.GetUserInventory(userId);

                Inventory.Clear();
                Cart.Clear();

                foreach (var item in userInventory)
                {
                    var bookDetailsDto = await _bookService.GetBookById(item.BookId);
                    Console.WriteLine($"Book: {bookDetailsDto.Title}, {bookDetailsDto.Author}, {bookDetailsDto.Genre}, {bookDetailsDto.Image}");
                    Book book = new (
                        bookDetailsDto.Id,
                        bookDetailsDto.Title,
                        bookDetailsDto.Author,
                        bookDetailsDto.Genre,
                        bookDetailsDto.Image,
                        bookDetailsDto.Stock
                    );

                    var inventoryItem = new Inventory
                    {
                        Id = item.Id,
                        UserId = item.UserId,
                        BookId = item.BookId,
                        DueDate = item.DueDate,
                        Book = book
                    };

                    Inventory.Add(inventoryItem);
                }
                OnPropertyChanged(nameof(Inventory));
                Console.WriteLine($"Loaded {userInventory.Count} inventory items for user {userId}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading inventory: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to load inventory.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ToggleInventory(Book book)
        {
            try
            {
                if (Cart.Contains(book))
                {
                    Cart.Remove(book);
                    book.IsAddButtonVisibleInventory = true;
                    book.IsRemoveButtonVisibleInventory = false;
                }
                else
                {
                    Cart.Add(book);
                    book.IsAddButtonVisibleInventory = false;
                    book.IsRemoveButtonVisibleInventory = true;
                }

                OnPropertyChanged(nameof(Cart));
                OnPropertyChanged(nameof(IsReturnButtonVisible));
            }  
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }





        private async Task ReturnBooks()
        {
            if (Cart.Count == 0)
            {
                await Shell.Current.DisplayAlert("Error", "No books to return.", "OK");
                return;
            }

            bool success = true;

            // Iterate over a copy of the cart to avoid modifying it during the loop
            foreach (var book in Cart.ToList())
            {
                var userInventory = await _inventoryService.GetUserInventory(UserId);
                var inv = userInventory.FirstOrDefault(inventory => inventory.BookId == book.Id);

                if (inv != null)
                {

                    try
                    {
                        // Remove the book from the inventory
                        bool returnSuccess = await _inventoryService.ReturnBook(inv.Id);
                        if (!returnSuccess)
                        {
                            success = false;
                            await Shell.Current.DisplayAlert("Error", $"Failed to remove book {book.Title} from inventory.", "OK");
                            continue;
                        }

                        // Update the stock of the book in the database
                        var updatedBook = new BookUpdateDto
                        {
                            Title = book.Title,
                            Author = book.Author,
                            Genre = book.Genre,
                            Image = book.Image,
                            Stock = book.Stock + 1
                        };

                        bool updateSuccess = await _bookService.UpdateBook(book.Id, updatedBook);
                        if (updateSuccess)
                        {
                            Inventory.Clear();
                            await GetInventoryAsync(UserId);
                            Notifier.NotifyInventoryUpdated();


                        }
                        else
                        {
                            success = false;
                            await Shell.Current.DisplayAlert("Error", $"Failed to update stock for book: {book.Title}", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                    }
                }
                else
                {
                    // If no inventory item found for this book, mark as failure
                    success = false;
                    await Shell.Current.DisplayAlert("Error", $"Book not found in inventory: {book.Title}", "OK");
                }
            }

            // Clear the Cart after processing all books
            Cart.Clear();
            OnPropertyChanged(nameof(IsReturnButtonVisible));

            if (success)
            {
                Notifier.NotifyInventoryUpdated();
                await Shell.Current.DisplayAlert("Success", "Books returned successfully!", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Some books could not be returned.", "OK");
            }
        }



        public void OnDisappearing()
        {          
            foreach (var book in Cart)
            {
                book.IsAddButtonVisibleInventory = true;
                book.IsRemoveButtonVisibleInventory = false;
            }
            Cart.Clear();
            OnPropertyChanged(nameof(IsReturnButtonVisible));


        }
        ~InventoryViewModel()
        {
            BookViewModel.Notifier.InventoryUpdated -= OnInventoryUpdated;
        }
    }
}
