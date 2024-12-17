namespace LibraryApp.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private readonly InventoryService _inventoryService;
        private readonly UserService _userService;
        private readonly BookService _bookService;

        public ObservableCollection<object> Items { get; set; } = new ObservableCollection<object>();
        public ICommand UpdateCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand AddCommand { get; }

        private string _selection;
        public string Selection
        {
            get => _selection;
            set
            {
                if (_selection != value)
                {
                    _selection = value;
                    OnPropertyChanged();
                    LoadItems();
                }
            }
        }

        public AdminViewModel()
        {
            Title = "Admin Page";
            _inventoryService = new InventoryService();
            _userService = new UserService();
            _bookService = new BookService();

            UpdateCommand = new Command<object>(OnUpdate);
            RemoveCommand = new Command<object>(OnRemove);
            AddCommand = new Command(OnAdd);
        }

        private async void LoadItems()
        {
            Items.Clear(); // Clear previous items before loading new ones

            if (Selection == "Users")
            {
                var users = await _userService.GetAllUsersAsync();
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        Items.Add(user); // Add each UserReadDto object
                    }
                }
            }
            else if (Selection == "Books")
            {
                var books = await _bookService.GetAllBooks();
                if (books != null)
                {
                    foreach (var book in books)
                    {
                        Items.Add(book); // Add each BookReadDto object
                    }
                }
            }
            else if (Selection == "Inventories")
            {
                var inventories = await _inventoryService.GetAllInventory();
                if (inventories != null)
                {
                    foreach (var inventory in inventories)
                    {
                        Items.Add(inventory); // Add each InventoryReadDto object
                    }
                }
            }
        }

        private async void OnUpdate(object item)
        {
            if (item is BookReadDto book)
            {
                // Show a modal to update book (book details pre-filled)
                var updatedTitle = await Shell.Current.DisplayPromptAsync("Update Book", "Edit the book title", initialValue: book.Title);
                var updatedAuthor = await Shell.Current.DisplayPromptAsync("Update Book", "Edit the author", initialValue: book.Author);
                var updatedGenre = await Shell.Current.DisplayPromptAsync("Update Book", "Edit the genre", initialValue: book.Genre);
                var updatedStock = await Shell.Current.DisplayPromptAsync("Update Book", "Edit the stock", initialValue: book.Stock.ToString());

                if (!string.IsNullOrWhiteSpace(updatedTitle) && !string.IsNullOrWhiteSpace(updatedAuthor) &&
                    !string.IsNullOrWhiteSpace(updatedGenre) && int.TryParse(updatedStock, out var stock))
                {
                    var bookUpdateDto = new BookUpdateDto
                    {
                        Title = updatedTitle,
                        Author = updatedAuthor,
                        Genre = updatedGenre,
                        Stock = stock
                    };

                    var success = await _bookService.UpdateBook(book.Id, bookUpdateDto);
                    if (success)
                    {
                        await Shell.Current.DisplayAlert("Success", "Book updated successfully.", "OK");
                        LoadItems(); // Reload items after updating
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Failed to update book.", "OK");
                    }
                }
            }
            else if (item is InventoryReadDto inventory)
            {
                // Show a modal to update inventory (inventory details pre-filled)
                var updatedDueDate = await Shell.Current.DisplayPromptAsync("Update Inventory", "Edit the due date (yyyy-MM-dd)", initialValue: inventory.DueDate?.ToString("yyyy-MM-dd"));

                if (!string.IsNullOrWhiteSpace(updatedDueDate) && DateTime.TryParse(updatedDueDate, out var dueDate))
                {
                    var inventoryUpdateDto = new InventoryUpdateDto { DueDate = dueDate };
                    var success = await _inventoryService.UpdateInventory(inventory.Id, inventoryUpdateDto);
                    if (success)
                    {
                        await Shell.Current.DisplayAlert("Success", "Inventory updated successfully.", "OK");
                        LoadItems(); // Reload items after updating
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Failed to update inventory.", "OK");
                    }
                }
            }
            else if (item is UserReadDto user)
            {
                // Show a modal to update user (user details pre-filled)
                var updatedFirstName = await Shell.Current.DisplayPromptAsync("Update User", "Edit first name", initialValue: user.FirstName);
                var updatedLastName = await Shell.Current.DisplayPromptAsync("Update User", "Edit last name", initialValue: user.LastName);
                var updatedEmail = await Shell.Current.DisplayPromptAsync("Update User", "Edit email", initialValue: user.Email);
                var updatedPassword = await Shell.Current.DisplayPromptAsync("Update User", "Edit password", initialValue: user.Password);

                if (!string.IsNullOrWhiteSpace(updatedFirstName) && !string.IsNullOrWhiteSpace(updatedLastName) &&
                    !string.IsNullOrWhiteSpace(updatedEmail) && !string.IsNullOrWhiteSpace(updatedPassword))
                {
                    var userUpdateDto = new UserUpdateDto
                    {
                        FirstName = updatedFirstName,
                        LastName = updatedLastName,
                        Email = updatedEmail,
                        Password = updatedPassword
                    };

                    var success = await _userService.UpdateUserAsync(user.Id, userUpdateDto);
                    if (success)
                    {
                        await Shell.Current.DisplayAlert("Success", "User updated successfully.", "OK");
                        LoadItems(); // Reload items after updating
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Failed to update user.", "OK");
                    }
                }
            }
        }

        private async void OnRemove(object item)
        {
            if (item is BookReadDto book)
            {
                var result = await _bookService.DeleteBook(book.Id);
                if (result)
                {
                    Items.Remove(book);
                    await Shell.Current.DisplayAlert("Success", "Book removed successfully.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to remove book.", "OK");
                }
            }
            else if (item is InventoryReadDto inventory)
            {
                var result = await _inventoryService.ReturnBook(inventory.Id);
                if (result)
                {
                    Items.Remove(inventory);
                    await Shell.Current.DisplayAlert("Success", "Inventory item removed successfully.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to remove inventory item.", "OK");
                }
            }
            else if (item is UserReadDto user)
            {
                var result = await _userService.DeleteUserAsync(user.Id);
                if (result)
                {
                    Items.Remove(user);
                    await Shell.Current.DisplayAlert("Success", "User removed successfully.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to remove user.", "OK");
                }
            }
        }

        private async void OnAdd()
        {
            if (Selection == "Books")
            {
                // Prompt for book details
                var bookTitle = await Shell.Current.DisplayPromptAsync("Add Book", "Enter book title:");
                var bookAuthor = await Shell.Current.DisplayPromptAsync("Add Book", "Enter book author:");
                var bookGenre = await Shell.Current.DisplayPromptAsync("Add Book", "Enter book genre:");
                var bookStock = await Shell.Current.DisplayPromptAsync("Add Book", "Enter book stock:");

                if (!string.IsNullOrWhiteSpace(bookTitle) && !string.IsNullOrWhiteSpace(bookAuthor) &&
                    !string.IsNullOrWhiteSpace(bookGenre) && int.TryParse(bookStock, out var stock))
                {
                    var newBook = new BookWriteDto
                    {
                        Title = bookTitle,
                        Author = bookAuthor,
                        Genre = bookGenre,
                        Stock = stock
                    };

                    var success = await _bookService.AddBook(newBook);
                    if (success)
                    {
                        await Shell.Current.DisplayAlert("Success", "Book added successfully!", "OK");
                        LoadItems(); // Reload items after adding
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Failed to add book.", "OK");
                    }
                }
            }
            else if (Selection == "Users")
            {
                // Prompt for user details
                var userFirstName = await Shell.Current.DisplayPromptAsync("Add User", "Enter user's first name:");
                var userLastName = await Shell.Current.DisplayPromptAsync("Add User", "Enter user's last name:");
                var userEmail = await Shell.Current.DisplayPromptAsync("Add User", "Enter user's email:");
                var userPassword = await Shell.Current.DisplayPromptAsync("Add User", "Enter user's password:");

                if (!string.IsNullOrWhiteSpace(userFirstName) && !string.IsNullOrWhiteSpace(userLastName) &&
                    !string.IsNullOrWhiteSpace(userEmail) && !string.IsNullOrWhiteSpace(userPassword))
                {
                    var newUser = new UserWriteDto
                    {
                        FirstName = userFirstName,
                        LastName = userLastName,
                        Email = userEmail,
                        Password = userPassword
                    };

                    var addedUser = await _userService.AddUserAsync(newUser);
                    if (addedUser != null)
                    {
                        await Shell.Current.DisplayAlert("Success", "User added successfully!", "OK");
                        LoadItems(); // Reload items after adding
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Failed to add user.", "OK");
                    }
                }
            }
            else if (Selection == "Inventories")
            {
                // Prompt for inventory details
                var inventoryBookId = await Shell.Current.DisplayPromptAsync("Add Inventory", "Enter book ID for inventory:");
                var inventoryDueDate = await Shell.Current.DisplayPromptAsync("Add Inventory", "Enter due date:");

                if (int.TryParse(inventoryBookId, out var bookId) && !string.IsNullOrWhiteSpace(inventoryDueDate))
                {
                    var newInventory = new InventoryWriteDto
                    {
                        BookId = bookId,
                        DueDate = DateTime.Parse(inventoryDueDate)
                    };

                    var success = await _inventoryService.BorrowBooks(newInventory);
                    if (success)
                    {
                        await Shell.Current.DisplayAlert("Success", "Inventory added successfully!", "OK");
                        LoadItems(); // Reload items after adding
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Failed to add inventory.", "OK");
                    }
                }
            }
        }
    }
}
