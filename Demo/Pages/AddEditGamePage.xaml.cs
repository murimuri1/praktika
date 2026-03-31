using Demo.Entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Demo.Pages
{
    public partial class AddEditGamePage : Page
    {
        private readonly Game _currentGame;

        public AddEditGamePage(Game game)
        {
            InitializeComponent();
            _currentGame = game ?? new Game();
            DataContext = _currentGame;

            // Загружаем списки для выпадающих меню
            CBGenre.ItemsSource = App.Context.Genre.ToList();
            CBDeveloper.ItemsSource = App.Context.Developer.ToList();
            CBPublisher.ItemsSource = App.Context.Publisher.ToList();

            if (game != null)
            {
                // Устанавливаем заголовок для режима ПРАВКИ
                this.Title = "РЕДАКТИРОВАНИЕ ИГРЫ";
                TxtHeader.Text = "РЕДАКТИРОВАНИЕ ИГРЫ";

                TBTitle.Text = game.Title;
                TBDescription.Text = game.Description;
                TBPrice.Text = game.Price.ToString();
                TBDiscount.Text = (game.Discount * 100).ToString();

                CBGenre.SelectedItem = game.Genre;
                CBDeveloper.SelectedItem = game.Developer;
                CBPublisher.SelectedItem = game.Publisher;
            }
            else
            {
                // Заголовок для режима ДОБАВЛЕНИЯ
                this.Title = "ДОБАВЛЕНИЕ ИГРЫ";
                TxtHeader.Text = "ДОБАВЛЕНИЕ ИГРЫ";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentGame.Title = TBTitle.Text;
                _currentGame.Description = TBDescription.Text;

                if (decimal.TryParse(TBPrice.Text, out decimal price))
                    _currentGame.Price = price;

                if (double.TryParse(TBDiscount.Text, out double discount))
                    _currentGame.Discount = discount / 100.0;

                _currentGame.Genre = CBGenre.SelectedItem as Genre;
                _currentGame.Developer = CBDeveloper.SelectedItem as Developer;
                _currentGame.Publisher = CBPublisher.SelectedItem as Publisher;

                if (_currentGame.Id_Game == 0)
                    App.Context.Game.Add(_currentGame);

                App.Context.SaveChanges();
                MessageBox.Show("Информация сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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