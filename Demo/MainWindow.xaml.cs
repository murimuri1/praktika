using System;
using System.Windows;
using System.Windows.Controls;
using Demo.Pages;

namespace Demo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FrameMain.ContentRendered += MainFrame_ContentRendered;
            FrameMain.Navigate(new Pages.AuthorizationPage());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (FrameMain.CanGoBack) FrameMain.GoBack();
        }

        private void BtnStore_Click(object sender, RoutedEventArgs e)
        {
            FrameMain.Navigate(new GamePage());
        }

        private void BtnLibrary_Click(object sender, RoutedEventArgs e)
        {
            FrameMain.Navigate(new LibraryPage());
        }

        // Переход на страницу добавления игры из меню
        private void BtnAddGame_Click(object sender, RoutedEventArgs e)
        {
            FrameMain.Navigate(new AddEditGamePage(null));
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            if (FrameMain.Content == null) return;

            if (FrameMain.Content is Page currentPage)
            {
                this.Title = currentPage.Title;
            }

            string typeName = FrameMain.Content.GetType().Name;
            bool isAuthPage = typeName.Contains("uthorizationPage");

            if (isAuthPage)
            {
                HeaderPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                HeaderPanel.Visibility = Visibility.Visible;

                // Проверка прав администратора для кнопки ДОБАВИТЬ
                if (App.CurrentUser != null)
                {
                    TxtUserName.Text = App.CurrentUser.Login;
                    BtnAddGame.Visibility = (App.CurrentUser.Role_Id == 1) ? Visibility.Visible : Visibility.Collapsed;
                }

                BtnBack.Visibility = FrameMain.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}