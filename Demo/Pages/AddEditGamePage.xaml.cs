using Demo.Entities;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Demo.Pages
{
    public partial class AddEditGamePage : Page
    {
        private Game _currentGame;
        private string _imagePath;

        public AddEditGamePage()
        {
            InitializeComponent();
            LoadData();
        }

        public AddEditGamePage(Game game)
        {
            InitializeComponent();
            LoadData();

            _currentGame = game;

            TBTitle.Text = game.Title;
            TBDescription.Text = game.Description;
            TBPrice.Text = game.Price.ToString();
            TBDiscount.Text = (game.Discount ?? 0).ToString();

            CBGenre.SelectedItem = game.Genre?.Name_Genre;
            CBDeveloper.SelectedItem = game.Developer?.Name_Developer;
            CBPublisher.SelectedItem = game.Publisher?.Name_Publisher;

            _imagePath = game.Image;
        }

        private void LoadData()
        {
            CBGenre.ItemsSource = App.Context.Genre.Select(x => x.Name_Genre).ToList();
            CBDeveloper.ItemsSource = App.Context.Developer.Select(x => x.Name_Developer).ToList();
            CBPublisher.ItemsSource = App.Context.Publisher.Select(x => x.Name_Publisher).ToList();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            decimal price;
            if (!decimal.TryParse(TBPrice.Text, out price))
            {
                MessageBox.Show("Ошибка цены");
                return;
            }

            double discount = 0;
            double.TryParse(TBDiscount.Text, out discount);

            var genre = App.Context.Genre.First(x => x.Name_Genre == CBGenre.Text);
            var developer = App.Context.Developer.First(x => x.Name_Developer == CBDeveloper.Text);
            var publisher = App.Context.Publisher.First(x => x.Name_Publisher == CBPublisher.Text);

            if (_currentGame == null)
            {
                var game = new Game
                {
                    Title = TBTitle.Text,
                    Description = TBDescription.Text,
                    Price = price,
                    Discount = discount,
                    Genre_Id = genre.Id_Genre,
                    Developer_Id = developer.Id_Developer,
                    Publisher_Id = publisher.Id_Publisher,
                    Image = string.IsNullOrEmpty(_imagePath) ? "picture.png" : _imagePath
                };

                App.Context.Game.Add(game);
            }
            else
            {
                _currentGame.Title = TBTitle.Text;
                _currentGame.Description = TBDescription.Text;
                _currentGame.Price = price;
                _currentGame.Discount = discount;
                _currentGame.Genre_Id = genre.Id_Genre;
                _currentGame.Developer_Id = developer.Id_Developer;
                _currentGame.Publisher_Id = publisher.Id_Publisher;

                if (!string.IsNullOrEmpty(_imagePath))
                    _currentGame.Image = _imagePath;
            }

            App.Context.SaveChanges();

            NavigationService.Navigate(new GamePage());
        }

        private void BtnImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
                _imagePath = System.IO.Path.GetFileName(dialog.FileName);
        }
    }
}