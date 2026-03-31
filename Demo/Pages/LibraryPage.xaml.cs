using Demo.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Demo.Pages
{
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

        private void UpdateLibrary()
        {
            if (App.CurrentUser == null) return;

            try
            {
                // Получаем ID купленных игр
                var ownedGameIds = App.Context.UserGame
                    .Where(ug => ug.User_Id == App.CurrentUser.Id_User)
                    .Select(ug => ug.Game_Id)
                    .ToList();

                // Загружаем игры и сразу оборачиваем их в нашу ViewModel
                var myGames = App.Context.Game
                    .Where(g => ownedGameIds.Contains(g.Id_Game))
                    .ToList()
                    .Select(g => new LibraryItemViewModel { Game = g, IsInstalled = true }) // По умолчанию считаем всё установленным
                    .ToList();

                LibraryList.ItemsSource = myGames;

                TxtCount.Text = $"{myGames.Count} " + GetGamesWord(myGames.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки библиотеки: " + ex.Message);
            }
        }

        private string GetGamesWord(int count)
        {
            int lastDigit = count % 10;
            int lastTwoDigits = count % 100;

            if (lastTwoDigits >= 11 && lastTwoDigits <= 19) return "игр";
            if (lastDigit == 1) return "игра";
            if (lastDigit >= 2 && lastDigit <= 4) return "игры";
            return "игр";
        }

        // ==========================================
        // ОБРАБОТЧИКИ НАЖАТИЯ КНОПОК
        // ==========================================

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LibraryItemViewModel item)
            {
                MessageBox.Show($"Запуск игры {item.Game.Title}...", "Запуск", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Install_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LibraryItemViewModel item)
            {
                MessageBox.Show($"Игра {item.Game.Title} успешно установлена на ваш компьютер!", "Установка завершена", MessageBoxButton.OK, MessageBoxImage.Information);
                item.IsInstalled = true; // Триггер в XAML автоматически поменяет кнопки
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LibraryItemViewModel item)
            {
                var result = MessageBox.Show($"Вы действительно хотите удалить файлы игры {item.Game.Title} с этого ПК?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    item.IsInstalled = false; // Триггер в XAML автоматически поменяет кнопки
                }
            }
        }
    }

    // ==========================================
    // КЛАСС-ОБЕРТКА ДЛЯ УПРАВЛЕНИЯ СОСТОЯНИЕМ
    // ==========================================
    public class LibraryItemViewModel : INotifyPropertyChanged
    {
        public Game Game { get; set; }

        private bool _isInstalled;
        public bool IsInstalled
        {
            get { return _isInstalled; }
            set
            {
                _isInstalled = value;
                OnPropertyChanged();
            }
        }

        // Событие для обновления интерфейса в реальном времени
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}