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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace Computer_games_shop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            string login="";
            string role = "";
            Connection connection = new Connection();
            string password = connection.GetHashString(password_field.Password);
            //MessageBox.Show(login_field.Text + "\n" + password);
            //string password = password_field.Password;
            DataTable result = connection.cmd("Select * from users Where login='"+login_field.Text+"' and password='"+password+"'");
            if (result.Rows.Count > 0)
            {
                login = connection.select(result, "login");
                role = connection.select(result, "role");
            }
            else
                MessageBox.Show("Логин или пароль неверный");
            if (role=="User")
            {
                Catalog catalog = new Catalog(login);
                catalog.Show();
                this.Close();
            }
            if(role=="Admin")
            {
                AdminMain admin = new AdminMain(login);
                admin.Show();
                this.Close();
            }
        }
        private void cantLogIn_Click(object sender, RoutedEventArgs e)
        {
            Can_t_LogIn cantlogin = new Can_t_LogIn();
            cantlogin.Show();
            this.Close();
        }

        private void haventAccount_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            this.Close();
        }
    }
}
