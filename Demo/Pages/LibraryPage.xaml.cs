using Demo.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

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
                var ownedGameIds = App.Context.UserGame
                    .Where(ug => ug.User_Id == App.CurrentUser.Id_User)
                    .Select(ug => ug.Game_Id)
                    .ToList();

                var myGames = App.Context.Game
                    .Where(g => ownedGameIds.Contains(g.Id_Game))
                    .ToList()
                    .Select(g => new LibraryItemViewModel { Game = g, IsInstalled = false })
                    .ToList();

                LibraryList.ItemsSource = myGames;
                TxtCount.Text = $"{myGames.Count} " + GetGamesWord(myGames.Count);

                // ЛОГИКА ОТОБРАЖЕНИЯ ПУСТОГО ЭКРАНА
                if (myGames.Count == 0)
                {
                    EmptyStatePanel.Visibility = Visibility.Visible;
                    LibraryScrollViewer.Visibility = Visibility.Collapsed;
                }
                else
                {
                    EmptyStatePanel.Visibility = Visibility.Collapsed;
                    LibraryScrollViewer.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                (Application.Current.MainWindow as MainWindow)?.ShowToast("Ошибка: " + ex.Message, true);
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

        // Переход в магазин при пустой библиотеке
        private void GoToStore_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GamePage());
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LibraryItemViewModel item)
            {
                (Application.Current.MainWindow as MainWindow)?.ShowToast($"Запуск игры {item.Game.Title}...");
            }
        }

        private void Install_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LibraryItemViewModel item)
            {
                // ИСПОЛЬЗУЕМ КРАСИВЫЙ TOAST ВМЕСТО MESSAGEBOX
                (Application.Current.MainWindow as MainWindow)?.ShowToast($"Игра {item.Game.Title} установлена!");
                item.IsInstalled = true;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LibraryItemViewModel item)
            {
                var result = MessageBox.Show($"Удалить файлы {item.Game.Title} с компьютера?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    item.IsInstalled = false;
                    // Вызываем наше кастомное уведомление после успешного удаления
                    (Application.Current.MainWindow as MainWindow)?.ShowToast($"Игра {item.Game.Title} удалена!");
                }
            }
        }
    }

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}