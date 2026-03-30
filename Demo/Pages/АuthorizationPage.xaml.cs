using Demo.Entities;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Demo.Pages
{
    public partial class AuthorizationPage : Page
    {
        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = TBLogin.Text;
            var password = TBPassword.Password;

            var user = App.Context.User
                .FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль!",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            // сохраняем текущего пользователя
            App.CurrentUser = user;

            // переход на страницу игр
            NavigationService.Navigate(new GamePage());
        }
    }
}