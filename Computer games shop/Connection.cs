using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;

namespace Computer_games_shop
{
    public class Connection
    {
        string connectionstring = @"Data Source=localhost; Initial Catalog=computer_games_store; Integrated Security=True";
        public string GetHashString(string s)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(s);
            MD5CryptoServiceProvider CSP = new MD5CryptoServiceProvider();
            byte[] byteHash = CSP.ComputeHash(bytes);
            string hash = "";
            foreach (byte b in byteHash)
            {
                hash += string.Format("{0:x2}", b);
            }
            return hash;
        }
        public DataTable cmd(string cmd)
        {
            SqlConnection connection = new SqlConnection(connectionstring);            
            DataTable dataTable = new DataTable();
            connection.Open();
            SqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = cmd;
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            adapter.Fill(dataTable);
            connection.Close();
            return dataTable;            
        }
        public string select(DataTable table, string columnName)
        {
            DataRow[] data = table.Select();
            var value = data[0][columnName];
            string result = Convert.ToString(value);
            return result;

        }
        public byte[] img(string cmd)
        {
            SqlConnection connection = new SqlConnection(connectionstring);
            connection.Open();
            SqlCommand command = new SqlCommand(cmd, connection);
            SqlDataReader reader = command.ExecuteReader();
            byte[] data = {0,0};
            while (reader.Read())
            { 
                data = (byte[])reader.GetValue(0);
            }
            connection.Close();
            return data;
        }
        public string select_with_number(DataTable table, string columnName, int row)
        {
            DataRow[] data = table.Select();
            var value = data[row][columnName];
            string result = Convert.ToString(value);
            return result;
        }
        public void sendMessageToEmail(string mail, string message)
        {
            MailAddress from = new MailAddress("shigabiev.shamil@mail.ru", "Games Shop");
            MailAddress to = new MailAddress(mail);
            MailMessage m = new MailMessage(from, to);
            m.Subject = "Computer Games Shop";
            m.Body = message;
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
            smtp.Credentials = new NetworkCredential("shigabiev.shamil@mail.ru", "kjWr5s6ct7H62qbrWEu7");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}
