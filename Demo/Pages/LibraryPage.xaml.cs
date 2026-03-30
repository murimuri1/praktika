using Demo.Entities;
using System.Linq;
using System.Windows.Controls;

namespace Demo.Pages
{
    public partial class LibraryPage : Page
    {
        public LibraryPage()
        {
            InitializeComponent();

            var games = App.Context.UserGame
                .Where(x => x.User_Id == App.CurrentUser.Id_User)
                .Select(x => x.Game)
                .ToList();

            LibraryList.ItemsSource = games;
        }
    }
}