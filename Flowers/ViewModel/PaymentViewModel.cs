using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flowers.Models;
using Flowers.Services;
using System.Threading.Tasks;

namespace Flowers.ViewModel;

[QueryProperty(nameof(Order), "Order")]
public partial class PaymentViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private Order order;

    [ObservableProperty]
    private string cardNumber;

    [ObservableProperty]
    private string expiryDate;

    [ObservableProperty]
    private string cvv;

    [ObservableProperty]
    private string smsCode;

    [ObservableProperty]
    private float totalPrice;

    [ObservableProperty]
    private bool isSmsSent;

    public PaymentViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    partial void OnOrderChanged(Order value)
    {
        if (value != null)
        {
            TotalPrice = value.TotalPrice;
        }
    }

    [RelayCommand]
    public async Task SendSms()
    {
        if (string.IsNullOrWhiteSpace(CardNumber) || string.IsNullOrWhiteSpace(ExpiryDate) || string.IsNullOrWhiteSpace(Cvv))
        {
            await Toast.Make("Заполните данные карты", ToastDuration.Short, 16).Show();
            return;
        }

        // Логика отправки СМС

        await Task.Delay(1000);
        IsSmsSent = true;

        await Toast.Make("Код отправлен", ToastDuration.Short, 16).Show();
    }

    [RelayCommand]
    public async Task ConfirmPayment()
    {
        if (string.IsNullOrWhiteSpace(SmsCode))
        {
            await Toast.Make("Введите код из СМС", ToastDuration.Short, 16).Show();
            return;
        }

        // Проверка правильности СМС-кода
        if (SmsCode != "1234")
        {
            await Toast.Make("Неверный код из СМС", ToastDuration.Short, 16).Show();
            return;
        }

        await _databaseService.AddOrderAsync(Order);
        await Toast.Make("Заказ успешно оформлен!", ToastDuration.Short, 16).Show();
        await Shell.Current.GoToAsync("//FlyoutMain/Dashboard", true);
    }

    [RelayCommand]
    public async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
