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
using System.Security.Cryptography;

namespace Computer_games_shop
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        DateTime dateTime = DateTime.Now;
        public Registration()
        {
            InitializeComponent();
        }
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Connection connection = new Connection();
            DataTable logins = new DataTable();
            DataTable emails= new DataTable(); 
            string isfreelogin = "Select * from users where login='" + loginField.Text + "'";
            string isfreeemail = "Select * from users where email='" + emailField.Text + "'";
            logins = connection.cmd(isfreelogin);
            emails = connection.cmd(isfreeemail);
            if (loginField.Text == "" || emailField.Text == "" || passwordField.Password == "" || repeatpasswordField.Password == "")
                MessageBox.Show("Заполните все поля!!!");
            else
                if (logins.Rows.Count > 0)
                    MessageBox.Show("Данный логин уже занят");
                else
                    if (emails.Rows.Count > 0)
                    MessageBox.Show("Данный адрес электронной почты занят");
                else
                {
                    string[] dataemail = emailField.Text.Split('@');
                    if (dataemail.Length == 2)
                    {
                        string[] data2email = dataemail[1].Split('.');
                        if (data2email.Length == 2)
                        {
                            if (passwordField.Password == repeatpasswordField.Password)
                            {
                                string password = connection.GetHashString(passwordField.Password);
                                string registration = "Insert into users values ('User','" + loginField.Text + "','" + password + "','" + emailField.Text + "',0,'"+dateTime.ToString("yyyy/MM/dd") +"')";
                                DataTable registr = connection.cmd(registration);
                                string message = loginField.Text+", спасибо за регистрацию на нашем сервисе Game Shop";
                                connection.sendMessageToEmail(emailField.Text, message);
                                Catalog catalog = new Catalog(loginField.Text);
                                catalog.Show();
                                this.Close();
                            }
                            else
                                MessageBox.Show("Пароли не совпадают");
                        }
                        else
                            MessageBox.Show("Укажите почту в верном формате");
                    }
                    else
                        MessageBox.Show("Укажите почту в верном формате");
                }          
        }

        private void Logo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }
    }
}
