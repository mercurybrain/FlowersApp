using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Abstract;
using Flowers.Models;

namespace Flowers.ViewModel
{
    [QueryProperty(nameof(Bouquet), "Bouquet")]
    public partial class BouquetDetailViewModel : ObservableObject
    {
        private ISharingService _sharingService;

        [ObservableProperty]
        Bouquet bouquet;

        public BouquetDetailViewModel(ISharingService sharingService) {
            _sharingService = sharingService;
        }

        [RelayCommand]
        async Task AddToCart()
        {
            if (Bouquet != null) _sharingService.Add<CartItem>("NewCartItem", new CartItem(Bouquet, 1));
        }
    }
}
