using Demo.Entities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Demo.Pages
{
    public partial class PayPage : Page
    {
        private Game _game;

        public PayPage(Game game)
        {
            InitializeComponent();
            _game = game;
            GameName.Text = game.Title;
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CardNumber.Text) ||
                string.IsNullOrWhiteSpace(CVV.Text))
            {
                MessageBox.Show("Введите данные карты!");
                return;
            }

            App.Context.UserGame.Add(new UserGame
            {
                Game_Id = _game.Id_Game,
                User_Id = App.CurrentUser.Id_User,
                PurchaseDate = DateTime.Now
            });

            App.Context.SaveChanges();

            MessageBox.Show("Оплата успешна!");

            NavigationService.Navigate(new LibraryPage());
        }
    }
}