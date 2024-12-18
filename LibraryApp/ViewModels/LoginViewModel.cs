namespace LibraryApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;


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

        private bool _isLoginVisible = true;
        private bool _isRegisterVisible = false;

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

            // Initialize commands
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

        private async Task LoginAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            var user = users?.FirstOrDefault(u => u.Email == Email && u.Password == Password);

            if (user != null)
            {
                Preferences.Set("UserId", user.Id);
                UserId = user.Id;

                if (user.Admin)
                {
                    await Shell.Current.GoToAsync("//AdminPage");
                }
                else
                {
                    await Shell.Current.GoToAsync("//MainPage");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Login Failed", "Invalid username or password", "OK");
            }
        }

        private async Task RegisterAsync()
        {
            // Validation of input fields
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Email) ||
                string.IsNullOrEmpty(Password) || Password != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Error", "Please fill in all fields correctly", "OK");
                return;
            }

            if (!IsValidEmail(Email))
            {
                await Shell.Current.DisplayAlert("Invalid Email", "Please enter a valid email address.", "OK");
                return;
            }

            var users = await _userService.GetAllUsersAsync();
            foreach (var user in users)
            {
                var existingUser = await _userService.GetUserByIdAsync(user.Id);
                if (existingUser.Email == Email)
                {
                    // Notify user if email exists
                    await Shell.Current.DisplayAlert("Email Taken", "This email is already associated with an account. Please use a different email.", "OK");
                    return;
                }
            }

            var userDto = new UserWriteDto
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password
            };

            var addedUser = await _userService.AddUserAsync(userDto);
            if (addedUser != null)
            {
                await LoginAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Registration Failed", "An error occurred during registration", "OK");
            }
        }

        private static bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }
    }
}