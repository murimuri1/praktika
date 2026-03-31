using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Demo
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Entities.GameShopEntities2 Context
        { get; } = new Entities.GameShopEntities2();

        //свойство для хранения авторизованного пользователя
        public static Entities.User CurrentUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Инициализация CurrentUser
            CurrentUser = new Entities.User(); // Или загрузка из базы данных
        }

    }
}
