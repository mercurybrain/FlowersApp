using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Abstract;
using Flowers.Models;
using Flowers.Services;
using System.Collections.ObjectModel;

namespace Flowers.ViewModel
{
    public partial class AssembledListViewModel : ObservableObject
    {
        [ObservableProperty] private User user;

        [ObservableProperty] private ObservableCollection<AssembledBouquets> assembledBouquets;

        private readonly DatabaseService _databaseService;
        private ISharingService _sharingService;

        public AssembledListViewModel(DatabaseService databaseService, CartViewModel cartViewModel,
            ISharingService sharingService)
        {
            _databaseService = databaseService;
            _databaseService.DataUpdated += OnDataUpdated;
            _sharingService = sharingService;
            User = UserSession.Instance.CurrentUser;
            LoadBouquets();
        }

        private async void LoadBouquets()
        {
            var bouquetList = await _databaseService.GetAllAssembled();
            AssembledBouquets = new ObservableCollection<AssembledBouquets>(bouquetList);
        }

        private async void OnDataUpdated(object sender, EventArgs e)
        {
            LoadBouquets();
        }

        [RelayCommand]
        async Task AddToCart(AssembledBouquets bouquet)
        {
            if (bouquet != null) _sharingService.Add<CartAssembled>("NewCartItem", new CartAssembled(bouquet, 1));
        }
        [RelayCommand]
        async Task DeleteAssembled(AssembledBouquets bouquet) {
            if (bouquet != null) await _databaseService.DeleteAssembled(bouquet);
        }
    }
}
