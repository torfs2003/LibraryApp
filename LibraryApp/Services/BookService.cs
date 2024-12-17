namespace LibraryApp.Services
{
    public class BookService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseAddress;

        public BookService()
        {
            _baseAddress = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:8080/api/books/"
                : "http://localhost:8080/api/books/";

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseAddress)
            };
        }

        public async Task<List<BookReadDto>> GetAllBooks()
        {
            try
            {
                var response = await _httpClient.GetAsync("");
                response.EnsureSuccessStatusCode();

                var books = await response.Content.ReadFromJsonAsync<List<BookReadDto>>();
                return books ?? new List<BookReadDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching books: {ex.Message}");
                return new List<BookReadDto>();
            }
        }

        public async Task<BookReadDto> GetBookById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{id}");
                response.EnsureSuccessStatusCode();

                var book = await response.Content.ReadFromJsonAsync<BookReadDto>();
                if (book == null)
                {
                    throw new Exception("Book not found.");
                }

                return book;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching book with ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> AddBook(BookWriteDto newBook)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("", newBook);
                response.EnsureSuccessStatusCode();

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding new book: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateBook(int id, BookUpdateDto updatedBook)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{id}", updatedBook);
                response.EnsureSuccessStatusCode();

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating book with ID {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteBook(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{id}");
                response.EnsureSuccessStatusCode();

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting book with ID {id}: {ex.Message}");
                return false;
            }
        }
    }
}