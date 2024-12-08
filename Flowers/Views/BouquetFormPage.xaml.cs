using Flowers.Models;
using Flowers.ViewModel;

namespace Flowers.Views;

public partial class BouquetFormPage : ContentPage
{
    public BouquetFormPage(BouquetFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
