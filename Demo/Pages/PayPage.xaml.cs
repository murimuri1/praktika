using Demo.Entities;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Demo.Pages
{
    public partial class PayPage : Page
    {
        private readonly Game _currentGame;

        public PayPage(Game game)
        {
            InitializeComponent();
            _currentGame = game;

            // Заполнение данных о выбранной игре
            GameName.Text = _currentGame.Title;
            if (!string.IsNullOrEmpty(_currentGame.ImagePath))
            {
                try
                {
                    ImgGame.Source = new BitmapImage(new Uri(_currentGame.ImagePath, UriKind.RelativeOrAbsolute));
                }
                catch { }
            }
        }

        // ==========================================
        // УМНАЯ ИКОНКА БАНКОВСКОЙ КАРТЫ
        // ==========================================
        private void CardNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CardIcon == null) return;
            string text = CardNumber.Text;

            if (text.StartsWith("4"))
            {
                CardIcon.Text = "VISA";
                CardIcon.Foreground = new SolidColorBrush(Color.FromRgb(26, 159, 255)); // Синий
            }
            else if (text.StartsWith("5"))
            {
                CardIcon.Text = "Mastercard";
                CardIcon.Foreground = new SolidColorBrush(Color.FromRgb(255, 153, 0)); // Оранжевый
            }
            else if (text.StartsWith("2"))
            {
                CardIcon.Text = "МИР";
                CardIcon.Foreground = new SolidColorBrush(Color.FromRgb(163, 219, 89)); // Зеленый
            }
            else
            {
                CardIcon.Text = "💳";
                CardIcon.Foreground = new SolidColorBrush(Color.FromRgb(92, 109, 126)); // Стандартный серый
            }
        }

        /// <summary>
        /// Автоматическое форматирование ввода даты (ММ/ГГ)
        /// </summary>
        private void ExpiryDate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            string text = textBox.Text;

            if (text.Length == 2 && !text.Contains("/"))
            {
                textBox.Text = text + "/";
                textBox.SelectionStart = textBox.Text.Length;
            }
            else if (text.Length == 2 && text.Contains("/"))
            {
                textBox.Text = text.Substring(0, 1);
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CardNumber.Text) || CardNumber.Text.Length < 16)
            {
                (Application.Current.MainWindow as MainWindow)?.ShowToast("Введите полный номер карты (16 цифр)", true);
                return;
            }
            if (ExpiryDate.Text.Length < 5)
            {
                (Application.Current.MainWindow as MainWindow)?.ShowToast("Введите срок действия в формате ММ/ГГ", true);
                return;
            }
            if (CVV.Password.Length < 3)
            {
                (Application.Current.MainWindow as MainWindow)?.ShowToast("Введите корректный CVV код", true);
                return;
            }

            try
            {
                UserGame newPurchase = new UserGame
                {
                    User_Id = App.CurrentUser.Id_User,
                    Game_Id = _currentGame.Id_Game,
                    PurchaseDate = DateTime.Now
                };

                App.Context.UserGame.Add(newPurchase);
                App.Context.SaveChanges();

                (Application.Current.MainWindow as MainWindow)?.ShowToast("Оплата прошла успешно! Игра добавлена в библиотеку.");

                if (NavigationService.CanGoBack) NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                (Application.Current.MainWindow as MainWindow)?.ShowToast("Ошибка при оплате: " + ex.Message, true);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack) NavigationService.GoBack();
        }
    }
}