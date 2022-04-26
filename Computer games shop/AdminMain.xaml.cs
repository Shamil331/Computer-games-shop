using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Computer_games_shop
{
    /// <summary>
    /// Логика взаимодействия для AdminMain.xaml
    /// </summary>
    public partial class AdminMain : Window
    {
        string adminlogin = "";
        public AdminMain( string login)
        {
            InitializeComponent();
            adminlogin = login;
        }

        private void UsersList_Click(object sender, RoutedEventArgs e)
        {
            RedactUsers ru = new RedactUsers(adminlogin);
            ru.Show();
            this.Close();
        }

        private void GamesCatalog_Click(object sender, RoutedEventArgs e)
        {
            RedactGames RG = new RedactGames(adminlogin);
            RG.Show();
            this.Close();
        }

        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SettingBar.Visibility = Visibility.Visible;
        }

        private void Settings_MouseLeave(object sender, MouseEventArgs e)
        {
            SettingBar.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            Account account = new Account(adminlogin, this);
            account.Show();
            this.Visibility = Visibility.Hidden;
        } 
    }
}
