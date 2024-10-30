using CommunityToolkit.Maui;
using Flowers.Services;
using Flowers.ViewModel;
using Flowers.Views;
using Microsoft.Extensions.Logging;
using Flowers.Abstract;

namespace Flowers
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit(options =>
                {
                    options.SetShouldEnableSnackbarOnWindows(true);
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "Flowers.db3");
            builder.Services.AddSingleton(s => new DatabaseService(dbPath));
            builder.Services.AddSingleton<ISharingService, SharedService>();

            // Регистрация ViewModel
            builder.Services.AddSingleton<RegisterViewModel>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<DashboardViewModel>();
            builder.Services.AddSingleton<AssemblyViewModel>();
            builder.Services.AddSingleton<CartViewModel>();
            builder.Services.AddSingleton<ProfileViewModel>();
            builder.Services.AddSingleton<AdminPanelViewModel>();

            // Регистрация страниц
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<RegisterPage>();
            builder.Services.AddSingleton<DashboardPage>();
            builder.Services.AddSingleton<AssemblyPage>();
            builder.Services.AddSingleton<CartPage>();
            builder.Services.AddSingleton<ProfilePage>();
            builder.Services.AddSingleton<AdminPanelPage>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
