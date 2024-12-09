namespace LibraryApp.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Image { get; set; }
        public int Stock { get; set; }

        public bool IsAvailable { get; set; }
        public bool IsInCart { get; set; }
        public DateTime? DueDate { get; set; }


        public Book(int id, string title, string author, string genre, string image, int stock)
        {
            Id = id;
            Title = title;
            Author = author;
            Genre = genre;
            Image = image;
            IsAvailable = true;
            IsInCart = false;
            Stock = stock;
        }


        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Title) &&
                   !string.IsNullOrWhiteSpace(Author) &&
                   !string.IsNullOrWhiteSpace(Genre);
        }
    }
}
