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
    public partial class GamePage : Page
    {
        private List<Game> _allGames;

        public GamePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser != null && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Tag = App.CurrentUser.Role_Id == 1 ? "Visible" : "Collapsed";
            }

            try
            {
                var genres = App.Context.Genre.ToList();
                genres.Insert(0, new Genre { Id_Genre = 0, Name_Genre = "Все игры" });
                ListGenres.ItemsSource = genres;
                ListGenres.SelectedIndex = 0;

                _allGames = App.Context.Game.ToList();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                (Application.Current.MainWindow as MainWindow)?.ShowToast("Ошибка БД: " + ex.Message, true);
            }
        }

        private void ApplyFilters()
        {
            if (_allGames == null) return;

            var filtered = _allGames.AsEnumerable();

            if (ListGenres.SelectedItem is Genre selectedGenre && selectedGenre.Id_Genre != 0)
            {
                filtered = filtered.Where(g => g.Genre_Id == selectedGenre.Id_Genre);
            }

            if (!string.IsNullOrWhiteSpace(TBoxSearch.Text))
            {
                string search = TBoxSearch.Text.ToLower().Trim();
                filtered = filtered.Where(g => g.Title.ToLower().Contains(search));
            }

            var resultList = filtered.ToList();
            if (ComboSort != null)
            {
                switch (ComboSort.SelectedIndex)
                {
                    case 1: resultList = resultList.OrderBy(g => g.Price).ToList(); break;
                    case 2: resultList = resultList.OrderByDescending(g => g.Price).ToList(); break;
                }
            }

            GameList.ItemsSource = resultList;

            if (resultList.Count == 0)
            {
                EmptyStatePanel.Visibility = Visibility.Visible;
                GameList.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyStatePanel.Visibility = Visibility.Collapsed;
                GameList.Visibility = Visibility.Visible;
            }
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void ListGenres_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Game game)
                NavigationService.Navigate(new PayPage(game));
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Game game)
                NavigationService.Navigate(new AddEditGamePage(game));
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
                    (Application.Current.MainWindow as MainWindow)?.ShowToast("Игра удалена из магазина");
                    Page_Loaded(null, null);
                }
            }
        }
    }

    // ==========================================
    // КОНВЕРТЕРЫ ДЛЯ РЕЙТИНГА
    // ==========================================
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
                    if (starsCount <= 2) return new SolidColorBrush(Color.FromRgb(255, 76, 76));
                    if (starsCount == 3) return new SolidColorBrush(Color.FromRgb(255, 204, 0));
                    return new SolidColorBrush(Color.FromRgb(163, 219, 89));
                }
            }
            return new SolidColorBrush(Colors.Transparent);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}