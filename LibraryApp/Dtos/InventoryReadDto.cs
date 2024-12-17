namespace LibraryApp.Dtos
{
    public class InventoryReadDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int BookId { get; set; }


        public DateTime? DueDate { get; set; }
    }
}
