using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Abstract;
using Flowers.Models;
using Flowers.Services;
using Flowers.Views;

namespace Flowers.ViewModel;

public partial class CartViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly ISharingService _sharingService;

    [ObservableProperty] private ObservableCollection<CartItem> cartItems = new();
    [ObservableProperty] private ObservableCollection<CartAssembled> assembledBouquets = new();
    [ObservableProperty] private string cityStreetHouse;
    [ObservableProperty] private string floor;
    [ObservableProperty] private string apartment;
    [ObservableProperty] private string description;
    [ObservableProperty] private string paymentMethod;
    public bool IsCartEmpty => !CartItems.Any() && !AssembledBouquets.Any();

    public CartViewModel(DatabaseService databaseService, ISharingService sharingService)
    {
        _databaseService = databaseService;
        _sharingService = sharingService;
        _sharingService.ItemAdded += OnItemAdded;

        (CityStreetHouse, Floor, Apartment) = ProjectTools.ParseAddress(UserSession.Instance.CurrentUser.AddressDefault);
        PaymentMethod = "Наличными при получении";
    }

    // Логика для добавления товара в корзину
    private async void OnItemAdded(object sender, object e)
    {
        if (e is CartItem newItem)
        {
            var existingItem = CartItems.FirstOrDefault(item => item.SelectedBouquet.Id == newItem.SelectedBouquet.Id);
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
        else if (e is CartAssembled assembled)
        {
            var existingItem = AssembledBouquets.FirstOrDefault(item => item.SelectedBouquet.Id == assembled.SelectedBouquet.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += assembled.Quantity;
            }
            else
            {
                AssembledBouquets.Add(assembled);
            }
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(IsCartEmpty));
        }
    }

    public float TotalPrice =>
        CartItems.Sum(item => item.SelectedBouquet.Price * item.Quantity) +
        AssembledBouquets.Sum(item => item.SelectedBouquet.Price * item.Quantity);

    [RelayCommand]
    public async Task ClearCart()
    {
        CartItems.Clear();
        AssembledBouquets.Clear();
        OnPropertyChanged(nameof(TotalPrice));
        OnPropertyChanged(nameof(IsCartEmpty));
    }

    [RelayCommand]
    public async Task ConfirmOrder()
    {
        if (CartItems.Count == 0 && AssembledBouquets.Count == 0)
        {
            await Toast.Make("Корзина пуста. Добавьте товары для оформления заказа.", ToastDuration.Short, 16).Show();
            return;
        }

        if (string.IsNullOrWhiteSpace(CityStreetHouse))
        {
            await Toast.Make("Пожалуйста, заполните поле адреса.", ToastDuration.Short, 16).Show();
            return;
        }

        string addr = ProjectTools.FormatAddress(CityStreetHouse, Floor, Apartment);

        // Словарь для хранения всех букетов
        var bouquetQuantities = new Dictionary<string, int>();

        // Добавляем готовые букеты
        foreach (var item in CartItems)
        {
            var key = $"G-{item.SelectedBouquet.Id}";
            bouquetQuantities[key] = item.Quantity;
            Trace.WriteLine($"Готовый букет: {item.SelectedBouquet.Name}, Quantity: {item.Quantity}");
        }

        // Добавляем собранные букеты
        foreach (var assembled in AssembledBouquets)
        {
            var key = $"A-{assembled.SelectedBouquet.Id}";
            bouquetQuantities[key] = assembled.Quantity;
            Trace.WriteLine($"Собранный букет: {assembled.SelectedBouquet.Name}, Quantity: {assembled.Quantity}");
        }

        var order = new Order
        {
            OrderDate = DateTime.Now,
            Username = UserSession.Instance.CurrentUser.Username,
            DeliveryAddress = addr,
            BouquetQuantities = bouquetQuantities, // Уникальные ключи
            TotalPrice = TotalPrice,
            Description = "Оплата: " + PaymentMethod + Description == "" ? "" : Description 
        };
        if (PaymentMethod == "Картой при заказе")
        {
            Trace.WriteLine("Картой при оплате");
            // Переход на страницу оплаты
            var navigationParameter = new Dictionary<string, object> { { "Order", order } };
            await Shell.Current.GoToAsync("Cart/paymentdetails", navigationParameter);
            return;
        }

        await _databaseService.AddOrderAsync(order);

        if (UserSession.Instance.CurrentUser.AddressDefault == "Не указан")
        {
            User user = UserSession.Instance.CurrentUser;
            user.AddressDefault = order.DeliveryAddress;

            await _databaseService.UpdateUserAsync(user);
        }

        await Toast.Make("Заказ успешно оформлен!", ToastDuration.Short, 16).Show();

        CityStreetHouse = string.Empty;
        Apartment = string.Empty;
        Floor = string.Empty;

        ClearCartCommand.Execute(null);
        _databaseService.NotifyDataUpdated();
    }
    [RelayCommand]
    public async Task GoToDashboard()
    {
        await Shell.Current.GoToAsync("//FlyoutMain/Dashboard", true);
    }
}
