namespace LibraryApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool isBusy;
        private string title = "";
        private int _stock;
        private DateTime? _dueDate;
        protected int _userId;

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (isBusy == value)
                    return;
                isBusy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        public string Title
        {
            get => title;
            set
            {
                if (title == value)
                    return;
                title = value;
                OnPropertyChanged();
            }
        }

        public int Stock
        {
            get => _stock;
            set
            {
                _stock = value;
                OnPropertyChanged();
            }
        }

        public DateTime? DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                OnPropertyChanged();
            }
        }
        public int UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        public BaseViewModel()
        {
            // Initialize UserId from Preferences
            UserId = Preferences.Get("userId", 0);
        }
        public bool IsNotBusy => !IsBusy;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = null)
        {
            // Log the current and new value of the property
            Debug.WriteLine($"Setting property: {propertyName}, Old Value: {backingField}, New Value: {value}");

            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;

            // Log the successful set operation
            Debug.WriteLine($"Property {propertyName} set to: {value}");

            OnPropertyChanged(propertyName);
            return true;
        }

    }
}