﻿namespace LibraryApp.Dtos
{
    public class UserReadDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool Admin { get; set; }
    }
}
