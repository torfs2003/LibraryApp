namespace LibraryApp.Services
{
    public class InventoryService
    {
        HttpClient _httpClient;
        string _baseAddress;

        public InventoryService()
        {
            _baseAddress = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:8080/api/inventory/"
                : "http://localhost:8080/api/inventory/";

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseAddress)
            };
        }

        public async Task<List<InventoryReadDto>> GetAllInventory()
        {
            try
            {
                var response = await _httpClient.GetAsync("");
                response.EnsureSuccessStatusCode();

                var inventoryList = await response.Content.ReadFromJsonAsync<List<InventoryReadDto>>();
                return inventoryList ?? new List<InventoryReadDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all inventory: {ex.Message}");
                return new List<InventoryReadDto>();
            }
        }

        public async Task<List<InventoryReadDto>> GetUserInventory(int userId)
        {
            try
            {
                // Fetch the user's inventory from the API
                var inventoryList = await _httpClient.GetFromJsonAsync<List<InventoryReadDto>>($"user/{userId}");

                // Ensure a valid return value (handle nulls gracefully)
                return inventoryList ?? new List<InventoryReadDto>();
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP error fetching user's inventory: {httpEx.Message}");
                throw new Exception("Unable to fetch user's inventory. Please check your network or API configuration.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error fetching user's inventory: {ex.Message}");
                throw; // Re-throw the exception to let the ViewModel handle it if necessary
            }
        }



        public async Task<InventoryReadDto> GetInventoryById(int inventoryId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{inventoryId}");
                response.EnsureSuccessStatusCode();

                var inventory = await response.Content.ReadFromJsonAsync<InventoryReadDto>();
                return inventory;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching inventory by ID: {ex.Message}");
                return null;
            }
        }


        public async Task<bool> BorrowBooks(InventoryWriteDto inventoryDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("", inventoryDto);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding inventory: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateInventory(int inventoryId, InventoryUpdateDto inventoryDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{inventoryId}", inventoryDto);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating inventory: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> ReturnBook(int inventoryId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{inventoryId}");
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine($"Failed to delete inventory ID {inventoryId}: {response.ReasonPhrase}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error returning book: {ex.Message}");
                return false;
            }
        }
    }
}

