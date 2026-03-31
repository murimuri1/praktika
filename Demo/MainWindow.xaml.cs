using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Demo.Pages;

namespace Demo
{
    public partial class MainWindow : Window
    {
        private bool _isToastShowing = false;

        public MainWindow()
        {
            InitializeComponent();
            FrameMain.ContentRendered += MainFrame_ContentRendered;
            FrameMain.Navigate(new Pages.AuthorizationPage());
        }

        // ==========================================
        // МЕТОД ВЫЗОВА УВЕДОМЛЕНИЙ (TOAST)
        // ==========================================
        public async void ShowToast(string message, bool isError = false)
        {
            if (_isToastShowing) return; // Защита от спама уведомлениями
            _isToastShowing = true;

            ToastText.Text = message;

            // Настройка цветов: Красный для ошибок, Зеленый для успеха
            if (isError)
            {
                ToastNotification.Background = new SolidColorBrush(Color.FromRgb(255, 76, 76)); // Красный
                ToastText.Foreground = Brushes.White;
                ToastIcon.Foreground = Brushes.White;
                ToastIcon.Text = "✖";
            }
            else
            {
                ToastNotification.Background = new SolidColorBrush(Color.FromRgb(163, 219, 89)); // Зеленый
                ToastText.Foreground = new SolidColorBrush(Color.FromRgb(23, 26, 33)); // Темно-серый
                ToastIcon.Foreground = new SolidColorBrush(Color.FromRgb(23, 26, 33));
                ToastIcon.Text = "✓";
            }

            ToastNotification.Visibility = Visibility.Visible;

            // Анимация выезда снизу вверх (Плавная, с замедлением)
            DoubleAnimation slideIn = new DoubleAnimation(100, 0, TimeSpan.FromMilliseconds(400))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            ToastTransform.BeginAnimation(TranslateTransform.YProperty, slideIn);

            // Висит 3 секунды
            await Task.Delay(3000);

            // Анимация уезда вниз
            DoubleAnimation slideOut = new DoubleAnimation(0, 100, TimeSpan.FromMilliseconds(400))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            slideOut.Completed += (s, e) =>
            {
                ToastNotification.Visibility = Visibility.Hidden;
                _isToastShowing = false;
            };
            ToastTransform.BeginAnimation(TranslateTransform.YProperty, slideOut);
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