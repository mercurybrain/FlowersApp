using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Models;
using Flowers.Services;

[QueryProperty(nameof(Order), "Order")]
public partial class OrderDetailViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private Order order;

    [ObservableProperty]
    private ObservableCollection<string> statusOptions;

    [ObservableProperty]
    private string selectedStatus;

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public OrderDetailViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;

        // Инициализация команд
        SaveCommand = new RelayCommand(OnSave);
        CancelCommand = new RelayCommand(OnCancel);

        // Загрузка возможных статусов
        LoadStatusOptions();
    }

    private void LoadStatusOptions()
    {
        // Загружаем строки описаний для всех статусов
        var statuses = Enum.GetValues(typeof(OrderStatus))
            .Cast<OrderStatus>()
            .Select(s => s.GetDescription())
            .ToList();

        StatusOptions = new ObservableCollection<string>(statuses);

        // Устанавливаем текущий статус
        SelectedStatus = Order?.StatusDescription;
    }

    private async void OnSave()
    {
        if (Order == null) return;

        // Преобразуем строку обратно в OrderStatus
        var newStatus = Enum.GetValues(typeof(OrderStatus))
            .Cast<OrderStatus>()
            .FirstOrDefault(s => s.GetDescription() == SelectedStatus);

        Order.Status = newStatus;

        await _databaseService.UpdateOrderAsync(Order);
        await Toast.Make("Статус заказа изменён!", ToastDuration.Short, 16).Show();
    }

    private void OnCancel()
    {
        
    }
}
