namespace LibraryApp.ViewModels
{
    public partial class InventoryViewModel : BaseViewModel
    {
        InventoryService _inventoryService;
        BookService _bookService;

        public ObservableCollection<Book> Cart { get; set; } = new();
        public ObservableCollection<Inventory> Inventory { get; set; } = new();

        

        public bool IsReturnButtonVisible => Cart.Count > 0;

        public bool IsBookInCart(Book book)
        {
            return Cart.Contains(book);
        }

        public ICommand LogoutCommand { get; }
        public ICommand ReturnBookCommand { get; }
        public ICommand ToggleInventoryCommand { get; }
        public ICommand GetInventoryCommand { get; }

      
        public InventoryViewModel(InventoryService inventoryService, BookService bookService)
        {
            Title = "My Inventory";
            _inventoryService = inventoryService;
            _bookService = bookService;

            LogoutCommand = new Command(Logout);
            GetInventoryCommand = new Command(async () => await GetInventoryAsync(UserId));
            ToggleInventoryCommand = new Command<Inventory>(async (inventory) => await ToggleInventory(inventory, UserId));
            ReturnBookCommand = new Command(async () => await ReturnBooks());

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
                    Book book = new Book(
                        bookDetailsDto.Id,
                        bookDetailsDto.Title,
                        bookDetailsDto.Author,
                        bookDetailsDto.Genre,
                        bookDetailsDto.Image,
                        bookDetailsDto.Stock
                    );
                    Cart.Add(book);

                    var inventoryItem = new Inventory
                    {
                        Id = item.Id,
                        UserId = item.UserId,
                        BookId = item.BookId,
                        DueDate = item.DueDate,
                        book = book
                    };

                    Inventory.Add(inventoryItem);
                }

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



        private async Task ToggleInventory(Inventory inventory,int userId)
        {
            try
            {


                var userInventory = await _inventoryService.GetUserInventory(userId);
                foreach (var inv in userInventory)
                {
                    var bookDetailsDto = await _bookService.GetBookById(inv.BookId);

                    var book = new Book(
                        bookDetailsDto.Id,
                        bookDetailsDto.Title,
                        bookDetailsDto.Author,
                        bookDetailsDto.Genre,
                        bookDetailsDto.Image,
                        bookDetailsDto.Stock
                    );

                    // Check if the book is already in the Cart
                    if (Cart.Contains(book))
                    {
                        // If the book is already in the cart, remove it (indicating it's being returned)
                        Cart.Remove(book); // Remove the book from the cart
                        book.IsAddButtonVisibleInventory = true;  // Show the "Add to Cart" button
                        book.IsRemoveButtonVisibleInventory = false; // Hide "Remove from Cart" button
                    }
                    else
                    {
                        // Add the book to the cart
                        Cart.Add(book);  // Add the book to the cart
                        book.IsAddButtonVisibleInventory = false; // Hide the "Add to Cart" button
                        book.IsRemoveButtonVisibleInventory = true;  // Show "Remove from Cart" button
                    }

                    // Notify UI about the changes
                    OnPropertyChanged(nameof(Cart));  // Update the Cart UI
                    OnPropertyChanged(nameof(IsReturnButtonVisible)); // Update visibility of the return button
                }

            }

                
            catch (Exception ex)
            {
                // Handle potential errors gracefully
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }





        private async Task ReturnBooks()
        {
            if (Cart.Count == 0)
            {
                return;
            }

            bool success = true;

            foreach (var book in Cart.ToList())
            {
                // Find the matching inventory item
                var inventoryItem = Inventory.FirstOrDefault(i => i.BookId == book.Id);

                if (inventoryItem != null)
                {
                    // Attempt to return the book
                    bool bookSuccess = await _inventoryService.ReturnBook(inventoryItem.Id);
                    if (!bookSuccess)
                    {
                        success = false;
                        await Shell.Current.DisplayAlert("Error", $"Failed to return book: {book.Title}", "OK");
                        continue; // Skip this book and move to the next
                    }

                    // Increment stock
                    book.Stock += 1;

                    // Update visibility in UI
                    book.IsAddButtonVisibleInventory = true;
                    book.IsRemoveButtonVisibleInventory = false;

                    // Remove the returned inventory item
                    Inventory.Remove(inventoryItem);
                }
            }

            // Clear the cart after processing
            Cart.Clear();

            // Notify UI
            OnPropertyChanged(nameof(Cart));
            OnPropertyChanged(nameof(IsReturnButtonVisible));

            if (success)
            {
                await Shell.Current.DisplayAlert("Success", "Books returned successfully!", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Some books could not be returned.", "OK");
            }
        }


        public void OnDisappearing()
        {
            // Reset the buttons and cart when the page is leaving
            Cart.Clear();
            foreach (var book in Cart)
            {
                book.IsAddButtonVisibleInventory = true;
                book.IsRemoveButtonVisibleInventory = false;
            }
        }
    }
}
