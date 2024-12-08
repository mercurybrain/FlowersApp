using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Flowers.Models;
using Flowers.Services;
using Flowers.ViewModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Abstract;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;
using Flowers.Views;

namespace Flowers.ViewModel;

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty] private User user;

    [ObservableProperty] private ObservableCollection<Bouquet> bouquets;

    private readonly DatabaseService _databaseService;
    private ISharingService _sharingService;

    public DashboardViewModel(DatabaseService databaseService, CartViewModel cartViewModel,
        ISharingService sharingService)
    {
        _databaseService = databaseService;
        _databaseService.DataUpdated += OnDataUpdated;
        _sharingService = sharingService;
        User = UserSession.Instance.CurrentUser;
        LoadBouquets();
    }

    // Асинхронная загрузка букетов
    private async void LoadBouquets()
    {
        var bouquetList = await _databaseService.GetAllBouquetsAsync();
        Bouquets = new ObservableCollection<Bouquet>(bouquetList);
    }

    private async void OnDataUpdated(object sender, EventArgs e)
    {
        LoadBouquets();
    }

    [RelayCommand]
    async Task AddToCart(Bouquet bouquet)
    {
        if (bouquet != null) _sharingService.Add<CartItem>("NewCartItem", new CartItem(bouquet, 1));
    }
    [RelayCommand]
    async Task NavigateToDetail(Bouquet bouquet) {
        await Shell.Current.GoToAsync($"//Dashboard/Details", true, new Dictionary<string, object> {
                        {"Bouquet", bouquet}
                    });
    }
}