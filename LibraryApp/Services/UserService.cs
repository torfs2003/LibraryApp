namespace LibraryApp.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseAddress;

        public UserService()
        {
            _baseAddress = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:8080/api/users/"
                : "http://localhost:8080/api/users/";

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseAddress)
            };
        }

        // Get all users
        public async Task<List<UserReadDto>> GetAllUsersAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<UserReadDto>>("");
            }
            catch
            {
                return null;
            }
        }

        // Get a user by ID
        public async Task<UserReadDto> GetUserByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserReadDto>(id.ToString());
            }
            catch
            {
                return null;
            }
        }

        // Add a new user
        public async Task<UserReadDto> AddUserAsync(UserWriteDto userDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("", userDto);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserReadDto>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        // Update a user
        public async Task<bool> UpdateUserAsync(int id, UserUpdateDto userDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(id.ToString(), userDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Delete a user
        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(id.ToString());
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}