using Demo.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private List<Game> _games;

        public GamePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser != null)
            {
                if (Application.Current.MainWindow != null)
                {
                    Application.Current.MainWindow.Tag = App.CurrentUser.Role_Id == 1 ? "Visible" : "Collapsed";
                }
            }

            try
            {
                _games = App.Context.Game.ToList();
                GameList.ItemsSource = _games;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_games == null) return;
            string search = TBoxSearch.Text.ToLower().Trim();
            GameList.ItemsSource = _games.Where(g => g.Title.ToLower().Contains(search)).ToList();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_games == null) return;
            var list = _games.ToList();

            switch (ComboSort.SelectedIndex)
            {
                case 1:
                    list = list.OrderBy(g => g.Price).ToList();
                    break;
                case 2:
                    list = list.OrderByDescending(g => g.Price).ToList();
                    break;
            }

            GameList.ItemsSource = list;
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Game game)
            {
                NavigationService.Navigate(new PayPage(game));
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Game game)
            {
                NavigationService.Navigate(new AddEditGamePage(game));
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Game game)
            {
                var result = MessageBox.Show($"Вы действительно хотите удалить игру '{game.Title}'?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    App.Context.Game.Remove(game);
                    App.Context.SaveChanges();
                    Page_Loaded(null, null);
                }
            }
        }

        private void AddGame_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditGamePage(null));
        }
    }

    // ==========================================================
    // КОНВЕРТЕРЫ ДЛЯ ОТОБРАЖЕНИЯ ЗВЕЗДОЧЕК И ИХ ЦВЕТА
    // ==========================================================

    public class RatingToStarsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string ratingText)
            {
                var match = Regex.Match(ratingText, @"\d+([.,]\d+)?");
                if (match.Success && double.TryParse(match.Value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double rating))
                {
                    int starsCount = (int)Math.Round(rating);
                    starsCount = Math.Max(0, Math.Min(5, starsCount));
                    return new string('★', starsCount);
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }

    public class RatingToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string ratingText)
            {
                var match = Regex.Match(ratingText, @"\d+([.,]\d+)?");
                if (match.Success && double.TryParse(match.Value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double rating))
                {
                    int starsCount = (int)Math.Round(rating);
                    if (starsCount <= 2) return new SolidColorBrush(Color.FromRgb(255, 76, 76)); // Красный
                    if (starsCount == 3) return new SolidColorBrush(Color.FromRgb(255, 204, 0)); // Желтый
                    return new SolidColorBrush(Color.FromRgb(163, 219, 89)); // Зеленый
                }
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}