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
            Routing.RegisterRoute("Dashboard/Details", typeof(BouquetDetailPage));

            Routing.RegisterRoute("bouquetediting", typeof(BouquetFormPage));
            Routing.RegisterRoute("orderdetail", typeof(OrderDetailPage));
            Routing.RegisterRoute("Cart/paymentdetails", typeof(PaymentPage));

            UpdateAdminPanelVisibility();

        }
        public void UpdateAdminPanelVisibility()
        {
            bool isAdmin = UserSession.Instance.CurrentUser?.Access == 777;
            bool isCourier = UserSession.Instance.CurrentUser?.Access == 555;
            var adminPanelItem = Items.FirstOrDefault(item => item.Route == "AdminPanel");
            if (adminPanelItem != null)
            {
                adminPanelItem.FlyoutItemIsVisible = isAdmin;
            }
            var courierPanelItem = Items.FirstOrDefault(item => item.Route == "Orders");
            if (courierPanelItem != null)
            {
                courierPanelItem.FlyoutItemIsVisible = isAdmin || isCourier;
            }
        }
        public void OnUserLoggedIn()
        {
            UpdateAdminPanelVisibility();
        }
    }
}
