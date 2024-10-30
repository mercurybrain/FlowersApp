using Flowers.Services;
using Flowers.Views;

namespace Flowers
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));

            UpdateAdminPanelVisibility();

        }
        public void UpdateAdminPanelVisibility()
        {
            bool isAdmin = UserSession.Instance.CurrentUser?.Username == "Admin";
            var adminPanelItem = Items.FirstOrDefault(item => item.Route == "AdminPanel");
            if (adminPanelItem != null)
            {
                adminPanelItem.FlyoutItemIsVisible = isAdmin;
            }
        }
        public void OnUserLoggedIn()
        {
            UpdateAdminPanelVisibility();
        }
    }
}
