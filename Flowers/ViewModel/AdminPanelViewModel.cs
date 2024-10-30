using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Models;
using Flowers.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Flowers.ViewModel
{
    public partial class AdminPanelViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty] private ObservableCollection<Bouquet> bouquets;
        [ObservableProperty] private ObservableCollection<User> users;
        [ObservableProperty] private ObservableCollection<Models.Flowers> flowers;
        [ObservableProperty] private ObservableCollection<Order> orders;

        public AdminPanelViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _databaseService.DataUpdated += OnDataUpdated;
            LoadData();
        }
        private async void OnDataUpdated(object sender, EventArgs e)
        {
            LoadData();
        }

        private async void LoadData()
        {
            Bouquets = new ObservableCollection<Bouquet>(await _databaseService.GetAllBouquetsAsync());
            Users = new ObservableCollection<User>(await _databaseService.GetAllUsersAsync());
            Flowers = new ObservableCollection<Models.Flowers>(await _databaseService.GetAllFlowersAsync());
            Orders = new ObservableCollection<Order>(await _databaseService.GetAllOrdersAsync());
        }

        [RelayCommand]
        private async Task DropDB() { 
            await _databaseService.ResetDatabaseAsync();
        }
        [RelayCommand]
        private async Task SeedDB() { 
            await _databaseService.SeedDataAsync();
        }

        [RelayCommand]
        private async Task AddBouquet()
        {
            // Логика для добавления букета
        }

        [RelayCommand]
        private async Task DeleteBouquet(Bouquet bouquet)
        {
            // Логика для удаления букета
        }

        [RelayCommand]
        private async Task AddUser()
        {
            // Логика для добавления пользователя
        }

        [RelayCommand]
        private async Task DeleteUser(User user)
        {
            if (user != null)
            {
                Users.Remove(user);
                await _databaseService.DeleteUserAsync(user);
            }
        }

        [RelayCommand]
        private async Task AddFlower()
        {
            // Логика для добавления цветка
        }

        [RelayCommand]
        private async Task DeleteFlower(Models.Flowers flower)
        {
            /*if (flower != null)
            {
                Flowers.Remove(flower);
                await _databaseService.DeleteFlowerAsync(flower);
            }*/
        }

        [RelayCommand]
        private async Task ChangeOrderStatus(Order order)
        {
            // Логика для изменения статуса заказа
        }

        [RelayCommand]
        private async Task DeleteOrder(Order order)
        {
            /*if (order != null)
            {
                Orders.Remove(order);
                await _databaseService.DeleteOrderAsync(order);
            }*/
        }

    }
}
