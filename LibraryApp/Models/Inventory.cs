namespace LibraryApp.Models
{
    public class Inventory
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int BookId { get; set; }

        public DateTime? DueDate { get; set; }

        public Book Book { get; set; }

        
    }
}
