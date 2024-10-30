using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Abstract;
using Flowers.Models;
using Flowers.Services;

namespace Flowers.ViewModel;

public partial class CartViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly ISharingService _sharingService;

    public CartViewModel(DatabaseService databaseService, ISharingService sharingService)
    {
        _databaseService = databaseService;
        _sharingService = sharingService;
        _sharingService.ItemAdded += OnItemAdded;
    }

    [ObservableProperty] private ObservableCollection<CartItem> cartItems = new();
    [ObservableProperty] private string cityStreetHouse;
    [ObservableProperty] private string floor;
    [ObservableProperty] private string apartment;

    public bool IsCartEmpty => !CartItems.Any();

    // Логика для добавления товара в корзину
    private async void OnItemAdded(object sender, object e)
    {
        if (e is CartItem newItem)
        {
            var existingItem = CartItems.FirstOrDefault(item => item.SelectedBouquet.Name == newItem.SelectedBouquet.Name);
            if (existingItem != null)
            {
                existingItem.Quantity += newItem.Quantity;
            }
            else
            {
                CartItems.Add(newItem);
            }
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(IsCartEmpty));
        }
    }

    public float TotalPrice => CartItems.Sum(item => item.SelectedBouquet.Price * item.Quantity);

    [RelayCommand]
    public async Task ClearCart()
    {
        CartItems.Clear();
        OnPropertyChanged(nameof(TotalPrice));
        OnPropertyChanged(nameof(IsCartEmpty));
    }

    [RelayCommand]
    public async Task ConfirmOrder()
    {
        if (CartItems.Count == 0)
        {
            await Toast.Make("Корзина пуста. Добавьте товары для оформления заказа.", ToastDuration.Short, 16).Show();
            return;
        }
        if (string.IsNullOrWhiteSpace(CityStreetHouse))
        {
            await Toast.Make("Пожалуйста, заполните поле адреса.", ToastDuration.Short, 16).Show();
            return;
        }

        string addr = string.IsNullOrEmpty(Floor)
                                          ? $"{CityStreetHouse}".Trim()
                                          : $"{CityStreetHouse}, подъезд и этаж: {Floor}, квартира: {Apartment}".Trim();

        var order = new Order
        {
            OrderDate = DateTime.Now,
            Username = UserSession.Instance.CurrentUser.Username,  // Здесь заменить на реального пользователя
            DeliveryAddress = addr,
            BouquetQuantities = CartItems.ToDictionary(item => item.SelectedBouquet, item => item.Quantity),
            TotalPrice = TotalPrice
        };

        await _databaseService.AddOrderAsync(order);
        await Toast.Make("Заказ успешно оформлен!", ToastDuration.Short, 16).Show();

        CityStreetHouse = string.Empty;
        Apartment = string.Empty;
        Floor = string.Empty;

        ClearCartCommand.Execute(null);
    }
    [RelayCommand]
    public async Task GoToDashboard()
    {
        await Shell.Current.GoToAsync("//FlyoutMain/Dashboard", true);
    }
}
