using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Abstract;
using Flowers.Models;
using Flowers.Services;
using Microsoft.Maui.Controls;

namespace Flowers.ViewModel;

public partial class OrdersListViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<Order> orders;

    [ObservableProperty]
    private Order selectedOrder;
    [ObservableProperty]
    private bool isModalVisible;
    [ObservableProperty]
    private ObservableCollection<string> statusOptions;

    [ObservableProperty]
    private string selectedStatus;

    public OrdersListViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;

        _databaseService.DataUpdated += OnDataUpdated;

        Orders = new ObservableCollection<Order>();
        StatusOptions = new ObservableCollection<string>(
            Enum.GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(s => s.GetDescription())
                .ToList()
        );
        LoadOrders();
    }

    private async void OnDataUpdated(object sender, EventArgs e)
    {
        LoadOrders();
    }

    public async void LoadOrders()
    {
        var orders = await _databaseService.GetUndeliveredOrdersAsync();

        foreach (var order in orders)
        {
            var user = await _databaseService.GetUserAsync(order.Username);
            order.Customer = user;

            if (order.BouquetQuantities != null && order.BouquetQuantities.Count > 0)
            {
                // Разделяем ключи на готовые и собранные букеты
                var readyBouquetIds = order.BouquetQuantities
                    .Where(kvp => kvp.Key.StartsWith("G-"))
                    .Select(kvp => int.Parse(kvp.Key.Substring(2)))
                    .ToList();

                var assembledBouquetIds = order.BouquetQuantities
                    .Where(kvp => kvp.Key.StartsWith("A-"))
                    .Select(kvp => int.Parse(kvp.Key.Substring(2)))
                    .ToList();

                // Загружаем готовые букеты
                var bouquets = await _databaseService.GetBouquetsByIdsAsync(readyBouquetIds);

                // Загружаем собранные букеты
                var assembledBouquets = await _databaseService.GetAssembledBouquetsByIdsAsync(assembledBouquetIds);

                // Объединяем данные
                var allBouquets = bouquets
                .Cast<IBouquet>()
                .Concat(assembledBouquets.Cast<IBouquet>())
                .GroupBy(b => b.Id)
                .ToDictionary(group => group.Key, group => group.First());

                foreach (var bouquet in allBouquets)
                {
                    // Загрузка магазина для букета
                    bouquet.Value.Store = await _databaseService.GetStoreById(bouquet.Value.StoreId);
                }

                order.DisplayBouquetQuantities = new Dictionary<IBouquet, int>();

                foreach (var kvp in order.BouquetQuantities)
                {
                    try
                    {
                        string keyPrefix = kvp.Key.Substring(0, 2); // Префикс "G-" или "A-"
                        int bouquetId = int.Parse(kvp.Key.Substring(2)); // ID букета
                        IBouquet bouquet = keyPrefix switch
                        {
                            "G-" => bouquets.FirstOrDefault(b => b.Id == bouquetId), // Ищем готовый букет
                            "A-" => assembledBouquets.FirstOrDefault(b => b.Id == bouquetId), // Ищем собранный букет
                            _ => null
                        };

                        if (bouquet == null)
                        {
                            Trace.WriteLine($"Не найден букет с Id: {bouquetId} (Key: {kvp.Key})");
                            bouquet = new Bouquet { Name = "Неизвестный букет", Price = 0 };
                        }

                        if (!order.DisplayBouquetQuantities.ContainsKey(bouquet))
                        {
                            order.DisplayBouquetQuantities[bouquet] = kvp.Value;
                        }
                        else
                        {
                            Trace.WriteLine($"Дубликат ключа: {bouquet.Name} (Id: {bouquet.Id})");
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"Ошибка при обработке ключа {kvp.Key}: {ex.Message}");
                    }
                }
                // Выводим отладочную информацию
                Trace.WriteLine("DisplayBouquetQuantities:");
                foreach (var kvp in order.DisplayBouquetQuantities)
                {
                    Trace.WriteLine($"Bouquet: {kvp.Key.Name}, Quantity: {kvp.Value}");
                    Trace.WriteLine($"Store: {kvp.Key.StoreId}, Name: {kvp.Key.Store.Name}, Address: {kvp.Key.Store.Address}");
                }
            }
        }

        Orders = new ObservableCollection<Order>(orders);
    }


    [RelayCommand]
    private void OpenChangeStatus(Order order)
    {
        SelectedOrder = order;
        SelectedStatus = order.StatusDescription;
        IsModalVisible = true;
    }
    [RelayCommand]
    private async Task SaveStatus()
    {
        if (SelectedOrder == null || string.IsNullOrEmpty(SelectedStatus)) return;

        var newStatus = Enum.GetValues(typeof(OrderStatus))
            .Cast<OrderStatus>()
            .FirstOrDefault(s => s.GetDescription() == SelectedStatus);

        SelectedOrder.Status = newStatus;
        await _databaseService.UpdateOrderAsync(SelectedOrder);

        IsModalVisible = false;
        _databaseService.NotifyDataUpdated();
    }
    [RelayCommand]
    private void CloseModal()
    {
        IsModalVisible = false;
    }
    [RelayCommand]
    private async Task AssignMe(Order order) {
        order.CourierUsername = UserSession.Instance.CurrentUser.Username;
        await _databaseService.UpdateOrderAsync(order);
        await Toast.Make("Заказ взят в работу, " + UserSession.Instance.CurrentUser.Username, ToastDuration.Short, 16).Show();
        _databaseService.NotifyDataUpdated();
    }
}
