using CommunityToolkit;
using CommunityToolkit.Maui.Alerts;
using Flowers.ViewModel;

namespace Flowers.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel viewModel)
	{
        InitializeComponent();
        BindingContext = viewModel;
    }
}