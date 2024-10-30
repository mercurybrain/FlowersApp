namespace Flowers
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            NavigateToLoginPage();
        }

        private async void NavigateToLoginPage()
        {
            // Задержка в 2 секунды перед переходом на страницу входа
            await Task.Delay(2000);

            // Переход на страницу входа
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
