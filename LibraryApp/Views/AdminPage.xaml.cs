namespace LibraryApp.Views;

public partial class AdminPage : ContentPage
{
	public AdminPage()
	{
		InitializeComponent();
	}

    private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        var selectedItem = picker.SelectedItem as string;
        if (BindingContext is AdminViewModel viewModel)
        {
            viewModel.Selection = selectedItem;
        }
    }
}