namespace LibraryApp.Dtos
{
    public class InventoryUpdateDto
    {
        public DateTime? DueDate { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
    }
}
