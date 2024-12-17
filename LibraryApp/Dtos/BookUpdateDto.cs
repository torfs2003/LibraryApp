﻿namespace LibraryApp.Dtos
{
    public class BookUpdateDto
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public string Genre { get; set; }

        public string? Image { get; set; }

        public int Stock { get; set; }
    }
}