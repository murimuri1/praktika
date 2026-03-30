using Demo.Entities;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Demo.Pages
{
    public partial class GamePage : Page
    {
        private List<Game> _games;

        public GamePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Настраиваем видимость кнопок в зависимости от роли (Tag будет читаться в XAML)
            // Предполагаем, что Role_Id == 1 — это Администратор
            if (App.CurrentUser != null)
            {
                if (App.CurrentUser.Role_Id == 1)
                {
                    // Для Админа: XAML скроет "Оплатить", но покажет "Редактировать"/"Удалить"
                    Application.Current.MainWindow.Tag = "Visible";
                }
                else
                {
                    // Для обычного Юзера: XAML покажет "Оплатить", но скроет "Редактировать"/"Удалить"
                    Application.Current.MainWindow.Tag = "Collapsed";
                }
            }

            _games = App.Context.Game.ToList();
            GameList.ItemsSource = _games;
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ЗАЩИТА: Если данные еще не загружены, ничего не делаем
            if (_games == null) return;

            string search = TBoxSearch.Text.ToLower();

            GameList.ItemsSource = _games
                .Where(g => g.Title.ToLower().Contains(search))
                .ToList();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ЗАЩИТА: Если данные еще не загружены, прерываем выполнение, чтобы избежать ArgumentNullException
            if (_games == null) return;

            var list = _games.ToList();

            if (ComboSort.SelectedIndex == 1)
                list = list.OrderBy(g => g.Price).ToList();

            if (ComboSort.SelectedIndex == 2)
                list = list.OrderByDescending(g => g.Price).ToList();

            GameList.ItemsSource = list;
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            var game = (sender as Button).DataContext as Game;

            if (game.IsOwned)
            {
                MessageBox.Show("Уже куплено!");
                return;
            }

            NavigationService.Navigate(new PayPage(game));
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var game = (sender as Button).DataContext as Game;
            NavigationService.Navigate(new AddEditGamePage(game));
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var game = (sender as Button).DataContext as Game;

            App.Context.Game.Remove(game);
            App.Context.SaveChanges();

            Page_Loaded(null, null);
        }

        private void Library_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LibraryPage());
        }
    }
}