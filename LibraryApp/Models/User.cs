namespace LibraryApp.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool Admin { get; set; } = false;

        public string Type => "User";
        public User(string firstName, string lastName, string email, string password, bool admin)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Admin = admin;
        }
    }
}
