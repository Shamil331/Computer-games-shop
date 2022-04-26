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
    /// Логика взаимодействия для RedactUsers.xaml
    /// </summary>
    public partial class RedactUsers : Window
    {
        string adminlogin = "";
        DataTable users;
        string selectedUserLogin = "";
        string selectedUserRole = "";
        string selectedUserEmail = "";
        string usersinfo = "";
        ComboBoxItem user = new ComboBoxItem();
        ComboBoxItem admin = new ComboBoxItem();
        Connection connection;
        DateTime dateTime= DateTime.Now;
        private void fillUsersList()
        {
            connection = new Connection();
            string cmd = "Select login, role from users";
            users= connection.cmd(cmd);
            int userscount=users.Rows.Count;
            for (int i=0; i<userscount; i++)
            {
                usersinfo= connection.select_with_number(users, "login", i)+":"+connection.select_with_number(users, "role",i);
                ListBoxItem userinfo = new ListBoxItem();
                userinfo.Content = usersinfo;
                string[] userlogin=usersinfo.Split(':');
                if (adminlogin == userlogin[0])
                    userinfo.IsEnabled = false;
                userslist.Items.Add(userinfo);
            }
        }
        public RedactUsers(string admin_login)
        {
            InitializeComponent();
            adminlogin = admin_login;
            fillUsersList();
            user.Tag = "User";
            user.Content = "User";
            admin.Tag = "Admin";
            admin.Content = "Admin";
            Roles.Items.Add(user);
            Roles.Items.Add(admin);
        }

        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SettingBar.Visibility = Visibility.Visible;
        }

        private void SettingBar_MouseLeave(object sender, MouseEventArgs e)
        {
            SettingBar.Visibility=Visibility.Hidden;
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            Account account = new Account(adminlogin, this);
            account.Show();
            this.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }
        private void userslist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            userEmailLabel.Content = "Email: ";
            userLoginLabel.Content = "Логин: ";
            Roles.IsEnabled = true;
            Change.IsEnabled = true;
            string[] logandrole = userslist.SelectedItem.ToString().Split(':');
            selectedUserLogin = logandrole[1];
            string[] temp = selectedUserLogin.Split(' ');
            selectedUserLogin = temp[1];
            selectedUserRole = logandrole[2];
            userLoginLabel.Content += selectedUserLogin;
            if (selectedUserRole == "User")
                Roles.SelectedItem = user;
            if (selectedUserRole =="Admin")
                Roles.SelectedItem = admin;
            string cmd= "Select email from users where login='"+selectedUserLogin+"'";
            DataTable useremail = connection.cmd(cmd);
            selectedUserEmail = connection.select(useremail, "email");
            userEmailLabel.Content += selectedUserEmail;
        }
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            string cmd="Update users set role='"+Roles.SelectionBoxItem.ToString()+"' where login='"+selectedUserLogin+"'";
            connection.cmd(cmd);
            string message = "Операции смена роли от: " + dateTime.ToString("dd.MM.yyyy") + ", " + dateTime.ToString("HH:mm:ss") + "<br/>Новая роль: <h1>" + Roles.SelectionBoxItem.ToString() + "</h1>";
            connection.sendMessageToEmail(selectedUserEmail, message);
            RedactUsers RU = new RedactUsers(adminlogin);
            RU.Show();
            this.Close();
        }

        private void Game_Shop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AdminMain AM = new AdminMain(adminlogin);
            AM.Show();
            this.Close();
        }
    }
}
