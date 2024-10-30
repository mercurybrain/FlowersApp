using Flowers.Services;
using Flowers.ViewModel;

namespace Flowers.Views;

public partial class LoginPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    public LoginPage(LoginViewModel viewModel, DatabaseService databaseService)
	{
		InitializeComponent();
        BindingContext = viewModel;

        _databaseService = databaseService;
    }
}