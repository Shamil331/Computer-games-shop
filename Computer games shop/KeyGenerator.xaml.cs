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
using System.Data;

namespace Computer_games_shop
{
    /// <summary>
    /// Логика взаимодействия для KeyGenerator.xaml
    /// </summary>
    public partial class KeyGenerator : Window
    {
        string gamename;
        string gamedeveloper;
        string adminlogin;
        int gameId;
        Connection connection = new Connection();
        DataTable game;
        public KeyGenerator(string Name_Of_Game, string Developer_Of_Game, string Login_Of_Admin)
        {
            gamename = Name_Of_Game;
            gamedeveloper = Developer_Of_Game;
            adminlogin = Login_Of_Admin;
            InitializeComponent();
            gameNameLabel.Content = gamename;
            gameDeveloperLabel.Content = gamedeveloper;
            string cmd = "Select * from games where name='"+gamename+"' and developer='"+gamedeveloper+"'";
            game=connection.cmd(cmd);
            gameKeysLabel.Content = connection.select(game, "quantity");
            gameId = Int32.Parse(connection.select(game, "id"));
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RedactGames RG = new RedactGames(adminlogin);
            RG.Show();
            this.Close();
        }

        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SettingBar.Visibility = Visibility.Visible;
        }

        private void SettingBar_MouseLeave(object sender, MouseEventArgs e)
        {
            SettingBar.Visibility = Visibility.Hidden;
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            Account AC = new Account(adminlogin, this);
            AC.Show();
            this.Visibility = Visibility.Hidden;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MW = new MainWindow();
            MW.Show();
            this.Close();
        }
        private string GenerateKey(string gamename)
        {
            string key="";
            string client = connection.select(game, "client");
            const string key_chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random rnd = new Random();
            if (client=="Steam")
            {
                for (int i=1; i<=17; i++)
                {
                    key += key_chars[rnd.Next(key_chars.Length)];
                    if (i==6 || i==12)
                    {
                        key += "-";
                        continue;
                    }
                }
            }
            if (client == "Origin" || client=="Uplay")
            {
                for (int i = 1; i <= 19; i++)
                {
                    key += key_chars[rnd.Next(key_chars.Length)];
                    if (i == 5 || i == 10 || i==15)
                    {
                        key += "-";
                        continue;
                    }
                }
            }
            return key;
        }
        int quantity = 0;
        private void AddKeys_Click(object sender, RoutedEventArgs e)    
        {
            string cmd;
            for (int i = 0; i < quantity; i++)
            {
                string key = GenerateKey(gamename);
                try
                {
                    cmd = "Insert into keys values (" + gameId + ",'" + key + "')";
                    connection.cmd(cmd);
                }
                catch
                {
                    continue;
                }
                cmd = "Update games set quantity=quantity+1";
                connection.cmd(cmd);
            }
            RedactGames RG = new RedactGames(adminlogin);
            RG.Show();
            this.Close();
        }

        private void KeysToAdd_TextChanged(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(KeysToAdd.Text, out quantity);
            if (quantity == 0 || quantity < 0 || quantity>50)
                Add.IsEnabled = false;
            else
                Add.IsEnabled = true;
        }
    }
}
