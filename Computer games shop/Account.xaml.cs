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
    /// Логика взаимодействия для Account.xaml
    /// </summary>
    public partial class Account : Window
    {
        Window previousWindow;
        string entereduserlogin;
        int balance;
        string password;
        Connection connection = new Connection();
        DataTable userdata;
        DateTime dateTime= DateTime.Now;
        public Account(string userlogin, Window window)
        {
            InitializeComponent();
            previousWindow = window;
            LoginBlock.Text = userlogin;
            entereduserlogin= userlogin;
            string cmd = "select * from users where login='"+userlogin+"'";
            userdata = connection.cmd(cmd);
            PasswordBlock.Text = connection.select(userdata, "password");
            password = connection.select(userdata, "password");
            EmailBlock.Text=connection.select(userdata, "email");
            BalanceBlock.Text = connection.select(userdata, "balance");
            balance = Convert.ToInt32(connection.select(userdata, "balance"));
            RoleBlock.Text = connection.select(userdata, "role");
            DateBlock.Text = connection.select(userdata, "registration_date");
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            previousWindow.Visibility = Visibility.Visible;
            this.Close();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginBlock.IsEnabled = true;
        }

        private void PasswordButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordBlock.IsEnabled = true;
        }

        private void MailButton_Click(object sender, RoutedEventArgs e)
        {
            EmailBlock.IsEnabled = true;
        }

        private void BalanceButton_Click(object sender, RoutedEventArgs e)
        {
            BalanceBlock.IsEnabled =true;
        }

        private void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            if(connection.cmd("Select * from users where login='"+LoginBlock.Text+"' and login!='"+entereduserlogin+"'").Rows.Count <= 0)
            {
                if (connection.cmd("Select * from users where email='" + EmailBlock.Text + "' and email!='"+ connection.select(connection.cmd("Select * from users where login='" + LoginBlock.Text + "'"), "email") + "'").Rows.Count <= 0)
                {
                    string cmd;
                    cmd = "Update users set login='" + LoginBlock.Text + "', email='" + EmailBlock.Text + "', balance=" + BalanceBlock.Text + " where login='" + entereduserlogin + "'";
                    if (password != PasswordBlock.Text)
                        cmd = "Update users set login='" + LoginBlock.Text + "', password='" + connection.GetHashString(PasswordBlock.Text) + "', email='" + EmailBlock.Text + "', balance=" + BalanceBlock.Text + " where login='" + entereduserlogin + "'";
                    connection.cmd(cmd);
                    string message = "<h1>Данные вашей учётной записи обновились.<br/>Новые данные</h1>\nЛогин: " + LoginBlock.Text + "<br/>Пароль: " + connection.GetHashString(PasswordBlock.Text) + "<br/>Почта: " + EmailBlock.Text;
                    connection.sendMessageToEmail(connection.select(userdata, "email"), message);
                    if (balance != Convert.ToInt32(BalanceBlock.Text))
                    {
                        message = "Операции изменения баланса от: " + dateTime.ToString("dd.MM.yyyy") + ", " + dateTime.ToString("HH:mm:ss") + "<br/><h1>Ваш баланс успешно изменён.</h1>Размер вашего счёта составляет: " + BalanceBlock.Text + "р.";
                        connection.sendMessageToEmail(connection.select(userdata, "email"), message);
                    }
                    Account reopen = new Account(LoginBlock.Text, previousWindow);
                    reopen.Show();
                    this.Close();
                }
                else MessageBox.Show("Данная почта уже занята");
            }
            else MessageBox.Show("Данный логин уже занят");
        }
    }
}
