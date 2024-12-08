namespace Flowers.Views;

public partial class OrderDetailPage : ContentPage
{
	public OrderDetailPage(OrderDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}