namespace LibraryApp.Models
{
    public class Inventory : BaseViewModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int BookId { get; set; }

        public DateTime? DueDate { get; set; }

        public string Type => "Inventory";

        public Book book { get; set; }

        
    }
}
