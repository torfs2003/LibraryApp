namespace LibraryApp.Dtos
{
    public class UserWriteDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool Admin { get; set; }
    }
}
