using Demo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для LibraryPage.xaml
    /// </summary>
    public partial class LibraryPage : Page
    {
        public LibraryPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLibrary();
        }

        /// <summary>
        /// Метод для загрузки только тех игр, которые принадлежат текущему пользователю
        /// </summary>
        private void UpdateLibrary()
        {
            if (App.CurrentUser == null) return;

            try
            {
                // Получаем ID всех игр, которые купил текущий пользователь
                var ownedGameIds = App.Context.UserGame
                    .Where(ug => ug.User_Id == App.CurrentUser.Id_User)
                    .Select(ug => ug.Game_Id)
                    .ToList();

                // Фильтруем основной список игр по этим ID
                var myGames = App.Context.Game
                    .Where(g => ownedGameIds.Contains(g.Id_Game))
                    .ToList();

                // Привязываем список к интерфейсу
                LibraryList.ItemsSource = myGames;

                // Обновляем счетчик игр
                TxtCount.Text = $"{myGames.Count} " + GetGamesWord(myGames.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки библиотеки: " + ex.Message);
            }
        }

        /// <summary>
        /// Вспомогательный метод для правильного склонения слова "игра"
        /// </summary>
        private string GetGamesWord(int count)
        {
            int lastDigit = count % 10;
            int lastTwoDigits = count % 100;

            if (lastTwoDigits >= 11 && lastTwoDigits <= 19) return "игр";
            if (lastDigit == 1) return "игра";
            if (lastDigit >= 2 && lastDigit <= 4) return "игры";
            return "игр";
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Game game)
            {
                // Оставил только чистое сообщение о запуске
                MessageBox.Show($"Запуск игры {game.Title}...", "Запуск", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}