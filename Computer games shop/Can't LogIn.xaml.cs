using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

namespace Computer_games_shop
{
    /// <summary>
    /// Логика взаимодействия для Can_t_LogIn.xaml
    /// </summary>
    public partial class Can_t_LogIn : Window
    {
        public Can_t_LogIn()
        {
            InitializeComponent();
        }
        private string check_code;
        Connection connection = new Connection();
        private string Code()
        {
            string code="";
            Random random = new Random();
            for (int i=0; i < 6; i++)
            {
                code += random.Next(0,9);
            }
            return code;
        }
        private void email_send_Click(object sender, RoutedEventArgs e)
        {
            string[] dataemail = email_field.Text.Split('@');
            if (dataemail.Length == 2)
            {
                string[] data2email = dataemail[1].Split('.');
                if (data2email.Length == 2)
                {
                    check_code = Code();
                    string message = "Ваш проверочный код: "+check_code+ "<h1>Никому не сообщайте его!!!</h1>";
                    try
                    {
                        connection.sendMessageToEmail(email_field.Text, message);
                    }
                    catch { MessageBox.Show("Электронной почты с таким адресом не существует"); Can_t_LogIn reopen= new Can_t_LogIn(); reopen.Show(); this.Close(); }
                    email_field.IsEnabled = false;
                    email_send.IsEnabled = false;
                    check_code_panel.Visibility = Visibility.Visible;
                    check_code_button.Visibility = Visibility.Visible;
                    PS.Visibility = Visibility.Visible;
                }
                else
                    MessageBox.Show("Введите корректный адрес электронной почты");
            }
            else
                MessageBox.Show("Введите корректный адрес электронной почты");
        }
        private void repass()
        {
            Random r = new Random();
            DateTime dateTime = DateTime.Now;            
            int n = r.Next(5, 10);
            string pass = "";
            while (pass.Length < n)
            {
                Char c = (char)r.Next(33, 125);
                if (Char.IsLetterOrDigit(c))
                    pass += c;
            }
            string message = "Запрос на восстановление пароля от: " + dateTime.ToString("dd.MM.yyyy") + ", " + dateTime.ToString("HH:mm:ss") + "<br/>Новый пароль: <h1>" + pass + "</h1>";
            connection.sendMessageToEmail(email_field.Text, message);
            pass = connection.GetHashString(pass);
            string SQLcmd = "Update users set password='"+pass+"' where email='" + email_field.Text + "'";
            connection.cmd(SQLcmd);
            MessageBox.Show("Новый пароль отправлен на вашу почту!");
        }
        private void send_code_button_Click(object sender, RoutedEventArgs e)
        {
            if (check_code_field.Text == check_code)
            {
                repass();
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
            }
            else
                MessageBox.Show("Проверочный код не совпадает");
        }

        private void Logo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }
    }
}
