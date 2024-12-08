using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Models;
using Flowers.Services;
using Flowers.Views;
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
        [ObservableProperty] private ObservableCollection<Store> stores;

        [ObservableProperty] private bool isAddingUser = false;
        [ObservableProperty] private User userToAdd;

        [ObservableProperty] private Order selectedOrder;
        [ObservableProperty] private bool isEditingOrder = false;
        public AdminPanelViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _databaseService.DataUpdated += OnDataUpdated;
            UserToAdd = new User();
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
            Stores = new ObservableCollection<Store>(await _databaseService.GetAllStoresAsync());
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
        private void CloseModals() { 
            IsAddingUser = false;
            IsEditingOrder = false;
        }

        [RelayCommand]
        private async Task AddBouquet()
        {
            await Shell.Current.GoToAsync($"bouquetediting", true, new Dictionary<string, object> {
                        {"Bouquet", new Bouquet()}
                    });
        }

        [RelayCommand]
        private async Task EditBouquet(Bouquet bouquet)
        {
            await Shell.Current.GoToAsync($"bouquetediting", true, new Dictionary<string, object> {
                        {"Bouquet", bouquet}
                    });
        }


        [RelayCommand]
        private async Task DeleteBouquet(Bouquet bouquet)
        {
            if (bouquet != null)
            {
                Bouquets.Remove(bouquet);
                await _databaseService.AddBouquetAsync(bouquet);
            }
        }

        [RelayCommand]
        private async Task AddUserFromAdmin()
        {
            IsAddingUser = true;
        }
        [RelayCommand]
        private async Task AddUser() {
            UserToAdd.Password = Encoder.ComputeHash(UserToAdd.Password, "SHA512", null);
            await _databaseService.SaveUserAsync(UserToAdd);

            await Toast.Make("Пользователь с именем '" + UserToAdd.Username + "' добавлен", ToastDuration.Short, 16).Show();
            LoadData();
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
        private async Task AddFlower(Models.Flowers flower)
        {
            if (flower != null)
            {
                Flowers.Remove(flower);
                await _databaseService.AddFlowerAsync(flower);
            }
        }

        [RelayCommand]
        private async Task DeleteFlower(Models.Flowers flower)
        {
            if (flower != null)
            {
                Flowers.Remove(flower);
                await _databaseService.DeleteFlowerAsync(flower);
            }
        }

        [RelayCommand]
        private async Task DeleteOrder(Order order)
        {
            if (order != null)
            {
                Orders.Remove(order);
                await _databaseService.DeleteOrderAsync(order);

                OnPropertyChanged(nameof(Orders));
            }
        }
        [RelayCommand]
        private async Task OpenOrderModal(Order order) { 
            IsEditingOrder = true;
            SelectedOrder = order;
        }
        [RelayCommand]
        private async Task EditOrder()
        {
            if (SelectedOrder == null) return;
            await _databaseService.UpdateOrderAsync(SelectedOrder);
            IsEditingOrder = false;
            LoadData();
        }
    }
}
