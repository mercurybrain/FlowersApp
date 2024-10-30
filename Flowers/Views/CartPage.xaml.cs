using Flowers.ViewModel;
using System.Globalization;

namespace Flowers.Views;

public partial class CartPage : ContentPage
{
	public CartPage(CartViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
public class FilterStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return new string(str.Where(c => char.IsLetter(c) || char.IsWhiteSpace(c)).ToArray());
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}