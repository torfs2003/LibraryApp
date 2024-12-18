namespace LibraryApp.Models
{
    public partial class Book :BaseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string? Image { get; set; }
        private int _stock;
        public new int Stock
        {
            get => _stock;
            set
            {
                if (SetProperty(ref _stock, value)) 
                {
                    OnPropertyChanged(nameof(IsAvailable));
                }
            }
        }


        public Book(int id, string title, string author, string genre, string image, int stock)
        {
            Id = id;
            Title = title;
            Author = author;
            Genre = genre;
            Image = image;
            Stock = stock;
        }


        private bool _isAddButtonVisibleBook = true;
        public bool IsAddButtonVisibleBook
        {
            get => _isAddButtonVisibleBook;
            set => SetProperty(ref _isAddButtonVisibleBook, value);
        }

        private bool _isRemoveButtonVisibleBook = false;
        public bool IsRemoveButtonVisibleBook
        {
            get => _isRemoveButtonVisibleBook;
            set => SetProperty(ref _isRemoveButtonVisibleBook, value);
        }

        private bool _isAddButtonVisibleInventory = true;
        public bool IsAddButtonVisibleInventory
        {
            get => _isAddButtonVisibleInventory;
            set => SetProperty(ref _isAddButtonVisibleInventory, value);
        }

        private bool _isRemoveButtonVisibleInventory = false;
        public bool IsRemoveButtonVisibleInventory
        {
            get => _isRemoveButtonVisibleInventory;
            set => SetProperty(ref _isRemoveButtonVisibleInventory, value);
        }

        public bool IsAvailable => Stock > 0;


        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Title) &&
                   !string.IsNullOrWhiteSpace(Author) &&
                   !string.IsNullOrWhiteSpace(Genre);
        }
    }
}
