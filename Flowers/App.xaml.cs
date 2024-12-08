using Flowers.Services;
using Flowers.ViewModel;
using Flowers.Views;
using System.Diagnostics;

namespace Flowers
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            

            MainPage = new AppShell();

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Trace.WriteLine($"Unhandled exception: {e.ExceptionObject}");
            };
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                Console.WriteLine($"Unobserved task exception: {e.Exception.Message}");
                e.SetObserved();
            };
        }
    }
}

