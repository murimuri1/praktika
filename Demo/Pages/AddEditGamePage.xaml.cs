using Demo.Entities;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Demo.Pages
{
    public partial class AddEditGamePage : Page
    {
        private readonly Game _currentGame;
        private string _selectedImagePath = "";

        public AddEditGamePage(Game game)
        {
            InitializeComponent();
            _currentGame = game ?? new Game();
            DataContext = _currentGame;

            // Загрузка списков
            CBGenre.ItemsSource = App.Context.Genre.ToList();
            CBDeveloper.ItemsSource = App.Context.Developer.ToList();
            CBPublisher.ItemsSource = App.Context.Publisher.ToList();

            if (game != null)
            {
                this.Title = "РЕДАКТИРОВАНИЕ ИГРЫ";
                TxtHeader.Text = "РЕДАКТИРОВАНИЕ ИГРЫ";

                // Заполнение основных полей
                TBTitle.Text = game.Title;
                TBDescription.Text = game.Description;
                TBPrice.Text = game.Price.ToString();
                TBDiscount.Text = (game.Discount * 100).ToString();

                // ЗАГРУЗКА РЕЙТИНГА: Вычисляем реальный средний рейтинг из таблицы Review
                var gameReviews = App.Context.Review.Where(r => r.Game_Id == game.Id_Game).ToList();
                if (gameReviews.Any())
                {
                    double avgRating = gameReviews.Average(r => (double)r.Rating);
                    // Выводим в формате 5.0
                    TBRating.Text = avgRating.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    TBRating.Text = ""; // Оценок нет
                }

                CBGenre.SelectedItem = game.Genre;
                CBDeveloper.SelectedItem = game.Developer;
                CBPublisher.SelectedItem = game.Publisher;

                // Загрузка обложки
                if (!string.IsNullOrEmpty(game.Image))
                {
                    try
                    {
                        ImgPreview.Source = new BitmapImage(new Uri(game.Image, UriKind.RelativeOrAbsolute));
                        _selectedImagePath = game.Image;
                    }
                    catch { }
                }
            }
            else
            {
                this.Title = "ДОБАВЛЕНИЕ ИГРЫ";
                TBRating.Text = "5.0"; // По умолчанию для новых игр
            }
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Изображения (*.jpg, *.png)|*.jpg;*.png|Все файлы (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                _selectedImagePath = dialog.FileName;
                ImgPreview.Source = new BitmapImage(new Uri(_selectedImagePath));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сбор текстовых данных
                _currentGame.Title = TBTitle.Text;
                _currentGame.Description = TBDescription.Text;
                _currentGame.Image = _selectedImagePath;

                if (decimal.TryParse(TBPrice.Text, out decimal price))
                    _currentGame.Price = price;

                if (double.TryParse(TBDiscount.Text, out double discount))
                    _currentGame.Discount = discount / 100.0;

                _currentGame.Genre = CBGenre.SelectedItem as Genre;
                _currentGame.Developer = CBDeveloper.SelectedItem as Developer;
                _currentGame.Publisher = CBPublisher.SelectedItem as Publisher;

                if (_currentGame.Id_Game == 0)
                    App.Context.Game.Add(_currentGame);

                // 1. СНАЧАЛА СОХРАНЯЕМ ИГРУ, чтобы в базе данных у нее появился Id_Game
                App.Context.SaveChanges();

                // 2. СОХРАНЯЕМ РЕЙТИНГ в таблицу Review
                string ratingStr = TBRating.Text.Replace(',', '.'); // Подстраховка для правильного формата числа
                if (double.TryParse(ratingStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double ratingValue))
                {
                    // Округляем до целых звезд
                    int intRating = (int)Math.Round(ratingValue);
                    if (intRating < 1) intRating = 1;
                    if (intRating > 5) intRating = 5;

                    // Ищем, ставил ли уже текущий Админ оценку этой игре
                    var adminReview = App.Context.Review.FirstOrDefault(r => r.Game_Id == _currentGame.Id_Game && r.User_Id == App.CurrentUser.Id_User);

                    if (adminReview != null)
                    {
                        adminReview.Rating = intRating; // Обновляем старую оценку
                    }
                    else
                    {
                        // Создаем новую системную оценку от Админа
                        Review newReview = new Review
                        {
                            Game_Id = _currentGame.Id_Game,
                            User_Id = App.CurrentUser.Id_User,
                            Rating = intRating,
                            Comment = "Официальная оценка администратора"
                        };
                        App.Context.Review.Add(newReview);
                    }

                    // Сохраняем отзыв в БД
                    App.Context.SaveChanges();
                }

                MessageBox.Show("Игра успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}