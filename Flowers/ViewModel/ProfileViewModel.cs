using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Abstract;
using Flowers.Models;
using Flowers.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Flowers.ViewModel
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        public User user;

        public ObservableCollection<Order> Orders { get; private set; } = new ObservableCollection<Order>();

        [ObservableProperty]
        public string selectedAddres;

        [ObservableProperty]
        private bool isChangeAddrVisible = false;

        [ObservableProperty] private string cityStreetHouse;
        [ObservableProperty] private string floor;
        [ObservableProperty] private string apartment;

        public bool IsOrdersEmpty
        {
            get
            {
                try
                {
                    return Orders == null || !Orders.Any();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Ошибка в IsOrdersEmpty: {ex.Message}");
                    return true;
                }
            }
        }

        public ProfileViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            _databaseService.DataUpdated += OnDataUpdated;

            LoadUserData();
            LoadOrderHistory();
        }



        private async void OnDataUpdated(object sender, EventArgs e)
        {
            LoadUserData();
            LoadOrderHistory();
        }

        private async void LoadUserData()
        {
            User = UserSession.Instance.CurrentUser;
            OnPropertyChanged(nameof(User));
        }

        private async void LoadOrderHistory()
        {
            if (User == null) return;

            var orders = await _databaseService.GetOrdersByUserAsync(User.Username);
            foreach (var order in orders)
            {
                var user = await _databaseService.GetUserAsync(order.CourierUsername);
                order.Courier = user;

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
                    }
                }
            }
            Orders = new ObservableCollection<Order>(orders);
            OnPropertyChanged(nameof(Orders)); OnPropertyChanged(nameof(IsOrdersEmpty));
        }

        [RelayCommand]
        private void OpenChangeAddr()
        {
            (CityStreetHouse, Floor, Apartment) = ProjectTools.ParseAddress(UserSession.Instance.CurrentUser.AddressDefault);
            IsChangeAddrVisible = true;
        }
        [RelayCommand]
        private async Task SaveAddr()
        {
            if (string.IsNullOrWhiteSpace(CityStreetHouse))
            {
                await Toast.Make("Пожалуйста, заполните поле адреса.", ToastDuration.Short, 16).Show();
                return;
            }

            string addr = string.IsNullOrEmpty(Floor)
                                              ? $"{CityStreetHouse}".Trim()
                                              : $"{CityStreetHouse}, подъезд и этаж: {Floor}, квартира: {Apartment}".Trim();

            User.AddressDefault = addr;
            await _databaseService.UpdateUserAsync(User);

            OnPropertyChanged(nameof(User));

            IsChangeAddrVisible = false;
        }
        [RelayCommand]
        private async Task DeleteAssembled() {
            await _databaseService.DropAssembledTable();

            await Toast.Make("Собранные букеты очищены", ToastDuration.Short, 16).Show();
        }
        [RelayCommand]
        private void CloseModal()
        {
            IsChangeAddrVisible = false;
        }
    }
}
