namespace LibraryApp.ViewModels
{
    public partial class BookViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly BookService _bookService;
        private readonly InventoryService _inventoryService;

        // Observable collections for books and cart
        public ObservableCollection<Book> Books { get; } = new();
        public ObservableCollection<Book> Cart { get; } = new();


        public ICommand LogoutCommand { get; }
        public ICommand GetBooksCommand { get; }
        public ICommand SaveBookCommand { get; }
        public ICommand ToggleBookCommand { get; }

        public bool IsSaveButtonVisible => Cart.Count > 0;

        public bool IsBookInCart(Book book)
        {
            return Cart.Contains(book);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("userId") && int.TryParse(query["userId"].ToString(), out int userId))
            {
                UserId = userId;
            }
        }

        // Constructor
        public BookViewModel(BookService bookService, InventoryService inventoryService)
        {
            Title = "Library Books";
            _bookService = bookService;
            _inventoryService = inventoryService;


            // Commands for various actions
            LogoutCommand = new Command(Logout);
            GetBooksCommand = new Command(async () => await GetBooksAsync());
            SaveBookCommand = new Command(async () => await SaveBooks());
            ToggleBookCommand = new Command<Book>(ToggleBook);

            Task.Run(async () => await GetBooksAsync());
        }

        private async void Logout()
        {
            Preferences.Remove("userEmail");
            Preferences.Remove("userPassword");

            // Optionally, clear the user token if you're using authentication tokens
            Preferences.Remove("userToken");


            await Shell.Current.GoToAsync($"//LoginPage?userId={UserId}");
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

                // Add each book to the Books collection
                foreach (var bookDetails in allBooks)
                {
                    var book = new Book(
                        bookDetails.Id,
                        bookDetails.Title,
                        bookDetails.Author,
                        bookDetails.Genre,
                        bookDetails.Image,  // Image can be null, so handle accordingly
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
                // If the book is already in the cart, remove it
                if (Cart.Contains(book))
                {
                    Cart.Remove(book); // Remove the book from the cart
                    book.IsAddButtonVisibleBook = true; // Show "Add to Cart" button
                    book.IsRemoveButtonVisibleBook = false; // Hide "Remove from Cart" button
                }
                else
                {
                    // Otherwise, add the book to the cart
                    Cart.Add(book); // Add the book to the cart
                    book.IsAddButtonVisibleBook = false; // Hide "Add to Cart" button
                    book.IsRemoveButtonVisibleBook = true; // Show "Remove from Cart" button
                }

                // Notify UI of changes
                OnPropertyChanged(nameof(Cart)); // Update the Cart UI
                OnPropertyChanged(nameof(IsSaveButtonVisible)); // Update visibility if needed
            }
            catch (Exception ex)
            {
                // Handle potential errors gracefully
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }






        // Save (Borrow) books in the cart
        private async Task SaveBooks()
        {
            if (Cart.Count == 0)
                return;

            var success = true;

            foreach (var book in Cart)
            {
                // Borrowing the book via the inventory service
                var inventoryItem = new InventoryWriteDto
                {
                    BookId = book.Id,
                    UserId = UserId,
                    DueDate = DateTime.Now.AddDays(30)
                };

                bool bookSuccess = await _inventoryService.BorrowBooks(inventoryItem);
                if (!bookSuccess)
                {
                    success = false;
                    break; // Exit the loop if one book fails
                }

                // Update the book stock (decrement by 1)
                var bookDetails = await _bookService.GetBookById(book.Id);
                if (bookDetails != null)
                {
                    // Decrement the stock value
                    var updatedBook = new BookUpdateDto
                    {
                        Title = bookDetails.Title,
                        Author = bookDetails.Author,
                        Genre = bookDetails.Genre,
                        Image = bookDetails.Image,
                        Stock = bookDetails.Stock - 1
                    };

                    // Update in the database
                    bool updateSuccess = await _bookService.UpdateBook(book.Id, updatedBook);
                    if (updateSuccess)
                    {
                        // Update the stock in the Book object in the collection
                        var bookInCollection = Books.FirstOrDefault(b => b.Id == book.Id);
                        if (bookInCollection != null)
                        {
                            bookInCollection.Stock = updatedBook.Stock; // This will notify the UI due to SetProperty
                            bookInCollection.IsAddButtonVisibleBook = true; // Reset to "Add" button
                            bookInCollection.IsRemoveButtonVisibleBook = false; // Reset to "Remove" button
                        }
                    }
                    else
                    {
                        success = false;
                        break; // Exit the loop if updating stock fails
                    }
                }
                else
                {
                    success = false;
                    break; // Exit the loop if the book details cannot be fetched
                }
            }

            // Show success or failure message after all books are processed
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
                    await Shell.Current.GoToAsync($"//InventoryPage?userId={UserId}");

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
        }

    }
}