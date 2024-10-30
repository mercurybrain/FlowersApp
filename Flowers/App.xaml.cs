using Flowers.Services;
using Flowers.ViewModel;
using Flowers.Views;

namespace Flowers
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            

            MainPage = new AppShell();
        }
    }
}
