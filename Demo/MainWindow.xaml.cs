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

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            if (FrameMain.Content == null) return;

            // IDE0019: Pattern matching кулланыла
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
                if (App.CurrentUser != null) TxtUserName.Text = App.CurrentUser.Login;
                BtnBack.Visibility = FrameMain.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}