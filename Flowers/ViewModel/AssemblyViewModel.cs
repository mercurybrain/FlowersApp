using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Abstract;
using Flowers.Models;
using Flowers.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Flowers.ViewModel
{
    public partial class AssemblyViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        private readonly ISharingService _sharedService;

        [ObservableProperty]
        private User user;

        [ObservableProperty]
        private ObservableCollection<Models.Flowers> availableFlowers; // Доступные цветы

        [ObservableProperty]
        private ObservableCollection<FlowerSelection> selectedFlowers; // Выбранные цветы для букета


        public AssemblyViewModel(DatabaseService databaseService, ISharingService sharedService)
        {
            _databaseService = databaseService;
            _sharedService = sharedService;
            User = UserSession.Instance.CurrentUser;

            selectedFlowers = new ObservableCollection<FlowerSelection>();
            LoadFlowers();
    }

        private async Task LoadFlowers()
        {
            var flowersList = await _databaseService.GetAllFlowersAsync();
            AvailableFlowers = new ObservableCollection<Models.Flowers>(flowersList);
        }

        // Команда для добавления цветка в сборочный список
        [RelayCommand]
        private void AddFlowerToBouquet(Models.Flowers flower)
        {
            var existingItem = SelectedFlowers.FirstOrDefault(f => f.Flower.Id == flower.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                var newFlowerSelection = new FlowerSelection(flower, 1);
                newFlowerSelection.QuantityChanged += OnQuantityChanged; // Подписка на изменения количества
                SelectedFlowers.Add(newFlowerSelection);
            }
            OnPropertyChanged(nameof(TotalPrice)); // Обновление общей стоимости
        }

        // Метод для обработки изменений количества
        private void OnQuantityChanged(object sender, System.EventArgs e)
        {
            OnPropertyChanged(nameof(TotalPrice)); // Обновляем общую стоимость при каждом изменении количества
            OnPropertyChanged(nameof(StoreId));
        }

        // Расчет общей стоимости букета
        public float TotalPrice => SelectedFlowers.Sum(item => item.Flower.Price * item.Quantity);
        private int StoreId => SelectedFlowers.FirstOrDefault().Flower.StoreId;

        // Команда для добавления собранного букета в корзину через _sharedService
        [RelayCommand]
        private async void AddBouquetToCart()
        {
            if (SelectedFlowers.Any())
            {
                var flowerDict = SelectedFlowers.ToDictionary(item => item.Flower.Name, item => item.Quantity);

                var allAssembled = await _databaseService.GetAllAssembled();
                int counter = allAssembled.Count;

                var uniqueBouquetName = $"Собранный букет {counter + 1}";

                var newBouquet = new AssembledBouquets
                {
                    Name = uniqueBouquetName,
                    Price = TotalPrice,
                    Flowers = flowerDict,
                    Icon = Array.Empty<byte>(),
                    StoreId = StoreId,
                };

                await _databaseService.AddAssembledAsync(newBouquet);

                Trace.WriteLine("Собранный букет сохранён: " + newBouquet.Name);

                // Добавляем собранный букет в корзину
                _sharedService.Add<CartAssembled>("NewCartItem", new CartAssembled(newBouquet, 1));

                SelectedFlowers.Clear();
                OnPropertyChanged(nameof(TotalPrice));
            }
        }


    }

    // Вспомогательный класс для хранения цветка и его количества в букете
    public class FlowerSelection : ObservableObject
    {
        public Models.Flowers Flower { get; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                SetProperty(ref _quantity, value);
                QuantityChanged?.Invoke(this, System.EventArgs.Empty); // Уведомляем об изменении количества
            }
        }

        public event EventHandler QuantityChanged;

        public FlowerSelection(Models.Flowers flower, int quantity)
        {
            Flower = flower;
            Quantity = quantity;
        }
    }
}
