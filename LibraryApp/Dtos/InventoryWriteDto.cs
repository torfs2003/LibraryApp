namespace LibraryApp.Dtos
{
    public class InventoryWriteDto
    {
        public DateTime? DueDate { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
    }
}
