﻿using Flowers.Models;
using Flowers.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui;
using static System.Net.Mime.MediaTypeNames;
using SQLite;
using System.Text.RegularExpressions;

namespace Flowers.ViewModel
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        public RegisterViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _phone;

        [RelayCommand]
        public async Task RegisterAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Phone))
                {
                    var toast = Toast.Make("Не все поля заполнены!", ToastDuration.Short, 16);

                    await toast.Show();
                    return;
                }
                if (!Regex.IsMatch(Phone, @"^\(\d{3}\)\d{3}-\d{2}-\d{2}$"))
                {
                    await Toast.Make("Введите корректный номер телефона!", ToastDuration.Short, 16).Show();
                    return;
                }
                else
                {
                    int generatedAcc = ProjectTools.GenerateRandomNumber();
                    if (Username == "Admin") {
                        generatedAcc = 777;
                    }
                    var user = new User
                    {
                        Username = Username,
                        Password = Encoder.ComputeHash(Password, "SHA512", null),
                        Phone = "+7" + Phone,
                        AddressDefault = "Не указан",
                        Access = generatedAcc
                    };

                    // Сохранение пользователя в базе данных
                    await _databaseService.SaveUserAsync(user);

                    var toast = Toast.Make("Регистрация успешна, теперь вы можете войти", ToastDuration.Short, 16);
                    await toast.Show();

                    await Shell.Current.GoToAsync("..", true);
                }
            }
            catch (SQLiteException ex) when (ex.GetType().ToString() == "UNIQUE constraint failed")
            {
                await Toast.Make("Ошибка: Уникальное ограничение нарушено. Возможно, данные уже существуют.", ToastDuration.Short, 16).Show();
            }
            catch (Exception ex) {
                await Toast.Make("Ошибка: " + ex.Message, ToastDuration.Short, 16).Show();
            }
        }
    }
}
