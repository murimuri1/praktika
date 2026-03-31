using Demo.Entities;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для PayPage.xaml
    /// </summary>
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
                catch
                {
                    // В случае ошибки загрузки фото используется заглушка по умолчанию
                }
            }
        }

        /// <summary>
        /// Автоматическое форматирование ввода даты (ММ/ГГ)
        /// </summary>
        private void ExpiryDate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;

            string text = textBox.Text;

            // Если введено 2 цифры и слэша еще нет — добавляем его
            if (text.Length == 2 && !text.Contains("/"))
            {
                textBox.Text = text + "/";
                textBox.SelectionStart = textBox.Text.Length;
            }
            // Если пользователь удаляет слэш, удаляем и цифру перед ним для удобства
            else if (text.Length == 2 && text.Contains("/"))
            {
                textBox.Text = text.Substring(0, 1);
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        /// <summary>
        /// Ограничение ввода: только цифры
        /// </summary>
        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения полей перед "оплатой"
            if (string.IsNullOrWhiteSpace(CardNumber.Text) || CardNumber.Text.Length < 16)
            {
                MessageBox.Show("Введите полный номер карты (16 цифр)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ExpiryDate.Text.Length < 5)
            {
                MessageBox.Show("Введите срок действия в формате ММ/ГГ (например, 10/27)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CVV.Password.Length < 3)
            {
                MessageBox.Show("Введите корректный 3-значный CVV код", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // ==========================================
            // РЕАЛЬНОЕ ДОБАВЛЕНИЕ ИГРЫ В БАЗУ ДАННЫХ
            // ==========================================
            try
            {
                // Создаем новую запись для таблицы UserGame
                UserGame newPurchase = new UserGame
                {
                    User_Id = App.CurrentUser.Id_User, // Берем ID авторизованного пользователя
                    Game_Id = _currentGame.Id_Game,    // Берем ID покупаемой игры
                    PurchaseDate = DateTime.Now        // Фиксируем дату и время покупки
                };

                // Добавляем запись в контекст Entity Framework
                App.Context.UserGame.Add(newPurchase);

                // Сохраняем изменения прямо в SQL Server
                App.Context.SaveChanges();

                // Сообщаем об успехе
                MessageBox.Show("Оплата прошла успешно! Игра добавлена в вашу библиотеку.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Возвращаемся обратно
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                // Если вдруг база данных недоступна или произошла ошибка
                MessageBox.Show("Ошибка при сохранении покупки в БД: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}