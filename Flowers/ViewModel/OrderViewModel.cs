using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Abstract;
using Flowers.Models;
using Flowers.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace Flowers.ViewModel
{
    public partial class OrderViewModel : ObservableObject
    {
        private readonly ISharingService _sharingService;
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private string addressPrimary;

        [ObservableProperty]
        private string addressSecondary;

        public ObservableCollection<CartItem> CartItems { get; }

        public float TotalPrice => CartItems.Sum(item => item.SelectedBouquet.Price * item.Quantity);

        public OrderViewModel(ISharingService sharingService, DatabaseService databaseService)
        {
            _sharingService = sharingService;
            _databaseService = databaseService;
            CartItems = new ObservableCollection<CartItem>(_sharingService.GetValue<List<CartItem>>("CartItems") ?? new List<CartItem>());
            IsOrderEnabled = CartItems.Any();
        }

        [ObservableProperty]
        private bool isOrderEnabled;

        [RelayCommand]
        private async Task ConfirmOrder()
        {
            if (!string.IsNullOrWhiteSpace(AddressPrimary))
            {
                var newOrder = new Order
                {
                    OrderDate = DateTime.Now,
                    Username = UserSession.Instance.CurrentUser.Username,
                    DeliveryAddress = $"{AddressPrimary}, {AddressSecondary}",
                    BouquetQuantities = CartItems.ToDictionary(item => item.SelectedBouquet, item => item.Quantity),
                    TotalPrice = TotalPrice,
                    Status = OrderStatus.OrderAccepted
                };

                await _databaseService.AddOrderAsync(newOrder);
                CartItems.Clear();
                await App.Current.MainPage.DisplayAlert("Заказ", "Ваш заказ успешно оформлен!", "OK");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", "Введите адрес доставки.", "OK");
            }
        }

        [RelayCommand]
        private void ClearCart()
        {
            CartItems.Clear();
            IsOrderEnabled = false;
        }
    }
}
