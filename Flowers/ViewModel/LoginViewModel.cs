using Flowers.Models;
using Flowers.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;
using Flowers.Views;
using Microsoft.Maui.Controls;

namespace Flowers.ViewModel
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        public ICommand RegisterCommand { get; }
        public ICommand LoginSuccCommand { get; }

        public LoginViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            RegisterCommand = new RelayCommand(NavigateToRegister);
            LoginSuccCommand = new RelayCommand(NavigateToDashboard);
        }

        private async void NavigateToRegister()
        {
            // Навигация на другую страницу через Shell
            await Shell.Current.GoToAsync("RegisterPage");
        }

        private async void NavigateToDashboard() {
            
        }

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        [RelayCommand]
        public async Task LoginAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    var toast = Toast.Make("Не все поля заполнены!", ToastDuration.Short, 16);

                    await toast.Show();
                    return;
                }

                var user = await _databaseService.GetUserAsync(Username);

                if (user != null && Encoder.VerifyHash(Password, "SHA512", user.Password))
                {
                    var toast = Toast.Make("Вы успешно вошли!", ToastDuration.Short, 16);
                    await toast.Show();
                    UserSession.Instance.CurrentUser = user;
                    ((AppShell)Shell.Current).OnUserLoggedIn();
                    await Shell.Current.GoToAsync("//FlyoutMain/Dashboard", true);
                    Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
                    /*await Shell.Current.GoToAsync($"//Dashboard", true, new Dictionary<string, object> {
                        {"User", user}
                    });*/
                }
                else
                {
                    var toast = Toast.Make("Неверные данные!", ToastDuration.Short, 16);
                    await toast.Show();
                }
            }
            catch (Exception ex) {
                await Toast.Make("Ошибка: " + ex.Message, ToastDuration.Short, 16).Show();
            }
        }
    }
}
