namespace LibraryApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _password;
        private string _confirmPassword;
        private bool _isLoginVisible = true;
        private bool _isRegisterVisible = false;

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public bool IsLoginVisible
        {
            get => _isLoginVisible;
            set => SetProperty(ref _isLoginVisible, value);
        }

        public bool IsRegisterVisible
        {
            get => _isRegisterVisible;
            set => SetProperty(ref _isRegisterVisible, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand SwitchToRegisterCommand { get; }
        public ICommand SwitchToLoginCommand { get; }

        public LoginViewModel(UserService userService)
        {
            _userService = userService;

            // Commands
            LoginCommand = new Command(async () => await LoginAsync());
            RegisterCommand = new Command(async () => await RegisterAsync());
            SwitchToRegisterCommand = new Command(SwitchToRegister);
            SwitchToLoginCommand = new Command(SwitchToLogin);
        }

        private void SwitchToRegister()
        {
            IsLoginVisible = false;
            IsRegisterVisible = true;
        }

        private void SwitchToLogin()
        {
            IsLoginVisible = true;
            IsRegisterVisible = false;
        }

        // Handle Login logic
        private async Task LoginAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            var user = users?.FirstOrDefault(u => u.Email == Email && u.Password == Password);

            if (user != null)
            {

                Preferences.Set("userId", user.Id);
                int userIdFromPreferences = Preferences.Get("userId", 0);
                Debug.WriteLine($"UserId saved to Preferences: {userIdFromPreferences}");

                Debug.WriteLine($"Setting UserId to: {user.Id}");
                UserId = user.Id;

                if (user.Admin)
                {
                    await Shell.Current.GoToAsync($"//AdminPage?userId={user.Id}");
                }
                else
                {
                    await Shell.Current.GoToAsync($"//MainPage?userId={user.Id}");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Login Failed", "Invalid username or password", "OK");
            }
        }

        // Handle Registration logic
        private async Task RegisterAsync()
        {
            // Validation of input fields
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Email) ||
                string.IsNullOrEmpty(Password) || Password != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Error", "Please fill in all fields correctly", "OK");
                return;
            }

            // Create a UserWriteDto for registration
            var userDto = new UserWriteDto
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password
            };

            // Call AddUserAsync to register the user
            var addedUser = await _userService.AddUserAsync(userDto);
            if (addedUser != null)
            {
                // Successfully registered. Automatically log the user in or proceed to the next step.
                await LoginAsync();
            }
            else
            {
                // Registration failed
                await Shell.Current.DisplayAlert("Registration Failed", "An error occurred during registration", "OK");
            }
        }
    }
}
