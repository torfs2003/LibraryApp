namespace LibraryApp.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly BookViewModel viewModel;
        public MainPage(BookViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            BindingContext = viewModel;

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.GetBooksAsync();
            
        }
    }


}

