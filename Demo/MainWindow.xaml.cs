using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Demo
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Подписываем именно Frame на событие отрисовки контента.
            FrameMain.ContentRendered += MainFrame_ContentRendered;

            // Начинаем работу со страницы авторизации.
            FrameMain.Navigate(new Pages.AuthorizationPage());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (FrameMain.CanGoBack)
            {
                FrameMain.GoBack();
            }
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            if (FrameMain.Content == null) return;

            // Получаем имя класса текущей страницы
            string typeName = FrameMain.Content.GetType().Name;

            // Проверка на страницу входа (игнорируем первую букву А из-за раскладки)
            bool isAuthPage = typeName.Contains("uthorizationPage");

            if (isAuthPage)
            {
                // На входе скрываем шапку
                HeaderPanel.Visibility = Visibility.Collapsed;
                BtnBack.Visibility = Visibility.Collapsed;
                TxtUserName.Text = "";
            }
            else
            {
                // Если пользователь вошел — показываем шапку
                HeaderPanel.Visibility = Visibility.Visible;

                // ОБНОВЛЯЕМ ИМЯ: Принудительно берем Login из App.CurrentUser
                if (App.CurrentUser != null)
                {
                    TxtUserName.Text = App.CurrentUser.Login;
                }
                else
                {
                    TxtUserName.Text = "Гость";
                }

                // Видимость кнопки "Назад"
                BtnBack.Visibility = FrameMain.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}