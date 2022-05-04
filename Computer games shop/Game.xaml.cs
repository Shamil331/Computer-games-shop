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
using System.IO;
using System.Drawing.Imaging;

namespace Computer_games_shop
{
    /// <summary>
    /// Логика взаимодействия для Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        Window previous_Window;
        string userLogin, gameName, gameGenre, gameClient, gamePlatform, gameDeveloper, gameDescription, gameSystem_requirements, gameDate;

        private void BuyButtom_Click(object sender, RoutedEventArgs e)
        {
            int balance = Convert.ToInt32(connection.select(connection.cmd("Select * from users where login='" + userLogin + "'"), "balance"));
            if (gameQuantity != 0)
            {
                if (balance >= gamePrice)
                {
                    connection.cmd("Update users set balance-=" + gamePrice + " where login='" + userLogin + "'");
                    DataTable key;
                    key = connection.cmd("Select top 1 * from keys where game_id='" + GameId + "'");
                    string keyvalue = connection.select(key, "value");
                    var id = connection.select(key, "id");
                    connection.cmd("Delete keys where id=" + id);
                    connection.cmd("Update games set quantity-=1 where id=" + GameId);
                    string userEmail = connection.select(connection.cmd("Select * from users where login='" + userLogin + "'"), "email");
                    QuantityLabel.Content = "Наличие: " + gameQuantity.ToString();
                    QuantityLabel.Foreground = SetColor(gameQuantity);
                    try { connection.sendMessageToEmail(userEmail, "<h1>Уважаемый покупатель!</h1>В интернет-магазине Computer games shop Вы приобрели товар " + gameName + "</br>Ваш ключ: <h1>" + keyvalue + "</h1>Спасибо за покупку!</br>С уважением, Computer games shop");
                        MessageBox.Show("Проверьте вашу почту");
                        previous_Window.Show();
                        this.Close();
                    }
                    catch { MessageBox.Show(keyvalue); }
                    connection.cmd("Insert into orders values ("+GameId+","+UserId+",'"+DateTime.Now+"')");
                }
                else MessageBox.Show("Недостаточно средств на счету");
            }
            else MessageBox.Show("Данного товара нет в наличии");
        }

        private void Game_Shop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            previous_Window.Show();
            this.Close();
        }

        private void Descriprion_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DescriptionLine.Visibility = Visibility.Hidden;
            S_RLine.Visibility = Visibility.Hidden;
            ActivationLine.Visibility = Visibility.Hidden;
            Descriprion.Foreground = new SolidColorBrush(Color.FromRgb(30,61,89));
            S_R.Foreground = new SolidColorBrush(Color.FromRgb(30, 61, 89));
            Activation.Foreground = new SolidColorBrush(Color.FromRgb(30, 61, 89));
            (sender as Label).Foreground = new SolidColorBrush(Color.FromRgb(255, 110, 64));
            if ((sender as Label).Name.ToString().ToLower() == "descriprion")
            {
                DescriptionLine.Visibility = Visibility.Visible;
                BottomTextBlock.Text = gameDescription;
            }
            if ((sender as Label).Name.ToString().ToLower() == "s_r")
            {
                S_RLine.Visibility = Visibility.Visible;
                BottomTextBlock.Text = gameSystem_requirements;
            }
            if ((sender as Label).Name.ToString().ToLower() == "activation")
            {
                ActivationLine.Visibility = Visibility.Visible;
                BottomTextBlock.Text = "После покупки вы мгновенно получите лицензионный ключ активации " + nameLabel.Text + " для " + gameClient + " на вашу почту.";
            }
        }

        private void FavouritesPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (connection.cmd("Select * from favourites where user_id=" + UserId + "and game_id=" + GameId).Rows.Count == 0)
            {
                connection.cmd("Insert into favourites values(" + UserId + "," + GameId + ")");
                FavouritesText.Content = "Удалить из избранного";
            }
            else
            { 
                connection.cmd("Delete favourites where user_id=" + UserId + " and game_id=" + GameId);
                FavouritesText.Content = "Добавить в избранное";
            }
        }

        int GameId, gamePrice, gameQuantity, UserId;
        Connection connection = new Connection();
        DataTable game = new DataTable();
        RedactGames RG = new RedactGames();
        private SolidColorBrush SetColor(int quantity)
        {
            if (quantity > 10)
                return Brushes.Green;
            else
                return Brushes.Red;
        }
        public Game(Window window, int gameId, string userlogin = null)
        {
            InitializeComponent();
            previous_Window = window;
            userLogin=userlogin;
            GameId = gameId;
            if (userlogin!=null) UserId = Convert.ToInt32(connection.select(connection.cmd("Select * from users where login='" + userLogin + "'"), "id"));
            string cmd = "Select * from games where id=" + GameId;
            game = connection.cmd(cmd);
            gameName = connection.select(game, "name").ToString();
            gameName = gameName.Replace('_', ' ');
            gamePrice = Convert.ToInt32(connection.select(game, "price"));
            gameGenre = connection.select(game, "genre");
            gameClient = connection.select(game, "client");
            gamePlatform = connection.select(game, "platform");
            gameDeveloper = connection.select(game, "developer");
            gameQuantity = Convert.ToInt32(connection.select(game,"quantity"));
            gameDescription = connection.select(game, "description");
            gameSystem_requirements = connection.select(game, "system_requirements");
            gameDate = connection.select(game, "release_date");
            gameDate = gameDate.Split(' ')[0];
            System.Drawing.Image gamePreview;
            BitmapImage imgsource = new BitmapImage();
            MemoryStream memorystream = new MemoryStream();
            try
            {
                byte[] BitImg = connection.img("Select preview from games where id="+GameId);
                gamePreview = RG.byteArrayToImage(BitImg);
                gamePreview = RG.ResizeImage(gamePreview, (int)GamePrview.Width, (int)GamePrview.Height);
                gamePreview.Save(memorystream, ImageFormat.Bmp);
                imgsource.BeginInit();
                imgsource.StreamSource = new MemoryStream(memorystream.ToArray());
                imgsource.EndInit();
            }
            catch
            {
                gamePreview = System.Drawing.Image.FromFile("C:/Users/79172/Desktop/Магазин компьютерных игр/blago.jpg");
                gamePreview.Save(memorystream, ImageFormat.Bmp);
                imgsource.BeginInit();
                imgsource.StreamSource = new MemoryStream(memorystream.ToArray());
                imgsource.EndInit();
            }
            GamePrview.Source = imgsource;
            nameLabel.Text = gameName;
            platoformLabel.Content += gamePlatform;
            clientLabel.Content += gameClient;
            GenreLabel.Content += gameGenre;
            DeveloperLabel.Content += gameDeveloper;
            DateLabel.Content += gameDate;
            QuantityLabel.Content += gameQuantity.ToString();
            QuantityLabel.Foreground = SetColor(gameQuantity);
            PriceLabel.Content += gamePrice.ToString() + "р.";
            if (userLogin == null)
            {
                FavouritesLine.Visibility = Visibility.Hidden;
                FavouritesPanel.Visibility = Visibility.Hidden;
                BuyButtom.IsEnabled = false;
            }
            else
            {
                //MessageBox.Show(connection.cmd("Select * from favourites where user_id=" + UserId + "and game_id=" + GameId).Rows.Count.ToString());
                FavouritesLine.Visibility = Visibility.Visible;
                FavouritesPanel.Visibility = Visibility.Visible;
                if (connection.cmd("Select * from favourites where user_id="+UserId+"and game_id="+GameId).Rows.Count!=0)
                {
                    FavouritesText.Content = "Удалить из избранного";
                }
                BuyButtom.IsEnabled=true;
                BuyButtom.MouseEnter += BuyButtom_MouseEnter;
                BuyButtom.MouseLeave += BuyButtom_MouseLeave;
            }
        }

        private void BuyButtom_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Button).Foreground = new SolidColorBrush(Color.FromRgb(30, 61, 89));
        }

        private void BuyButtom_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Button).Foreground = new SolidColorBrush(Color.FromRgb(255, 110, 64));
        }
    }
}
