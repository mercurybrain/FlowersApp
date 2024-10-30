using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Abstract;
using Flowers.Models;
using Flowers.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private int bouquetCounter = 1;

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
        }

        // Расчет общей стоимости букета
        public float TotalPrice => SelectedFlowers.Sum(item => item.Flower.Price * item.Quantity);

        // Команда для добавления собранного букета в корзину через _sharedService
        [RelayCommand]
        private void AddBouquetToCart()
        {
            if (SelectedFlowers.Any())
            {
                var flowerDict = SelectedFlowers.ToDictionary(item => item.Flower.Name, item => item.Quantity);

                var uniqueBouquetName = $"Собранный букет{bouquetCounter++}";

                var newBouquet = new Bouquet
                {
                    Name = uniqueBouquetName,
                    Price = TotalPrice,
                    Flowers = flowerDict,
                    Icon = Array.Empty<byte>()
                };

                // Добавляем собранный букет в общий сервис
                _sharedService.Add<CartItem>("NewCartItem", new CartItem(newBouquet, 1));

                // Очищаем сборочный список после добавления
                SelectedFlowers.Clear();
                OnPropertyChanged(nameof(TotalPrice)); // Обновляем общую стоимость после очистки
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
