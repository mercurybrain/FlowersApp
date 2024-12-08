using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Flowers.ViewModel;

namespace Flowers.Views;

public partial class AdminPanelPage : ContentPage
{
	public AdminPanelPage(AdminPanelViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    private void OnExpanderExpandedChanged(object sender, ExpandedChangedEventArgs e)
    {
        var expander = (Expander)sender;
        var image = (Image)((HorizontalStackLayout)expander.Header).Children[0];
        image.Source = e.IsExpanded ? "arrow_down.png" : "arrow_right.png";
    }
}