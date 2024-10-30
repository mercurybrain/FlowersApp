using Flowers.ViewModel;

namespace Flowers.Views;

public partial class AssemblyPage : ContentPage
{
	public AssemblyPage(AssemblyViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}