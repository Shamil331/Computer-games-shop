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
    /// Логика взаимодействия для Catalog.xaml
    /// </summary>
    public partial class Catalog : Window
    {
        string userlogin;
        string loginContent;
        Connection connection = new Connection();
        public Catalog(string login)
        {
            InitializeComponent();
            Login.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Visible;
            FavouritesPanel.Visibility = Visibility.Visible;
            userlogin = login;
            string cmd = "Select * from games";
            listGames(cmd);
        }
        public Catalog()
        {
            InitializeComponent();
            string cmd = "Select * from games";
            listGames(cmd);
            loginContent = Convert.ToString(Login.Content);
            userlogin = null;
        }
        RedactGames RG = new RedactGames();
        private Thickness priceLabelMargin(string platform, string price) //цена типа 1000р. имеет 6 элементов
        {
            Thickness th = new Thickness();
            th.Top = 0;
            th.Right = 0;
            th.Bottom = 0;
            int leftMargin=16*(6-price.Count());
            if (platform=="All")
                leftMargin += 241;
            if (platform == "Windows")
                leftMargin += 150;
            th.Left = leftMargin;
            return th;
        }
        private void listGames(string cmd)
        {
            GamesList.Children.Clear();
            DataTable games = connection.cmd(cmd);
            int gamesCount = games.Rows.Count;
            for (int i = 0; i < gamesCount; i++)
            {
                try
                {
                    int gameId;
                    gameId = Convert.ToInt32(connection.select_with_number(games, "id", i));
                    string gameName, gameYear, gameGenre, gamePrice, gameClient, gamePlatform;
                    gameName = connection.select_with_number(games, "name", i);
                    gameName = gameName.Replace('_', ' ');
                    gameYear = connection.select_with_number(games, "release_date", i);
                    string[] gameDate = gameYear.Split('.');
                    gameYear = gameDate[2];
                    gameDate = gameYear.Split(' ');
                    gameYear = gameDate[0];
                    gameGenre = connection.select_with_number(games, "genre", i);
                    gamePrice = connection.select_with_number(games, "price", i) + "р.";
                    gameClient = connection.select_with_number(games, "client", i);
                    gamePlatform = connection.select_with_number(games, "platform", i);
                    Label nameLabel = new Label();
                    nameLabel.Margin = new Thickness(5);
                    Label yearLabel = new Label();
                    Label genreLabel = new Label();
                    Label priceLabel = new Label();
                    priceLabel.MouseEnter += PriceLabel_MouseEnter;
                    priceLabel.MouseLeave += PriceLabel_MouseLeave;
                    Label clientLabel = new Label();
                    Label platfromLabel = new Label();
                    nameLabel.Content = gameName;
                    yearLabel.Content = gameYear;
                    genreLabel.Content = gameGenre;
                    priceLabel.Content = gamePrice;
                    clientLabel.Content = gameClient + " ";
                    platfromLabel.Content = " " + gamePlatform;
                    StackPanel panel = new StackPanel();// Панель для всех элементов (превьяшка и остальная инфа)
                    //System.Windows.Controls.StackPanel
                    //System.Windows.Input.MouseButtonEventArgs
                    panel.Tag = gameId;
                    panel.MouseDown += GameSelected;
                    Color br = new Color();
                    br.R = 245;
                    br.G = 240;
                    br.B = 225;
                    br.A = 100;
                    panel.Background = new SolidColorBrush(Color.FromArgb(100,255,255,255));
                    panel.Orientation = Orientation.Horizontal;
                    Image previewImage = new Image();
                    previewImage.Height = 210;
                    previewImage.Width = 375;
                    previewImage.Stretch = Stretch.Fill;
                    System.Drawing.Image gamePreview;
                    BitmapImage imgsource = new BitmapImage();
                    MemoryStream memorystream = new MemoryStream();
                    try
                    {
                        byte[] BitImg = connection.img("Select preview from games where name='" + connection.select_with_number(games, "name", i) + "'");
                        gamePreview = RG.byteArrayToImage(BitImg);
                        gamePreview = RG.ResizeImage(gamePreview, (int)previewImage.Width, (int)previewImage.Height);
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
                    previewImage.Source = imgsource;
                    panel.VerticalAlignment = VerticalAlignment.Center;
                    StackPanel gameInfoPanel = new StackPanel();
                    gameInfoPanel.Orientation = Orientation.Vertical;
                    gameInfoPanel.Children.Add(nameLabel);
                    gameInfoPanel.VerticalAlignment = VerticalAlignment.Center;
                    StackPanel YGPanel = new StackPanel();// Year and Genre Panel
                    YGPanel.Margin = new Thickness(5);
                    YGPanel.Orientation = Orientation.Horizontal;
                    YGPanel.Children.Add(yearLabel);
                    YGPanel.Children.Add(genreLabel);
                    gameInfoPanel.Children.Add(YGPanel);
                    StackPanel CPPanel = new StackPanel();// Client, Platfrom and Price Panel
                    CPPanel.Margin = new Thickness(5);
                    CPPanel.Orientation = Orientation.Horizontal;
                    CPPanel.Children.Add(clientLabel);
                    CPPanel.Children.Add(platfromLabel);
                    priceLabel.HorizontalAlignment = HorizontalAlignment.Right;
                    //double pricemargin = 531 - platfromLabel.ActualWidth - clientLabel.ActualWidth - priceLabel.ActualWidth - 400;
                    //pricemargin=-(200+priceLabel.ActualWidth);
                    priceLabel.Margin = priceLabelMargin(gamePlatform, gamePrice);
                    CPPanel.Children.Add(priceLabel);
                    gameInfoPanel.Children.Add(CPPanel);
                    panel.Margin = new Thickness(5);
                    panel.Children.Add(previewImage);
                    panel.Children.Add(gameInfoPanel);
                    GamesList.Children.Add(panel);
                }
                catch { continue; }
            }
        }

        private void PriceLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Label).Foreground = new SolidColorBrush(Color.FromRgb(30, 61, 89));
        }

        private void PriceLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Label).Foreground = new SolidColorBrush(Color.FromRgb(255, 193, 59));
        }

        private void GameSelected(object sender, EventArgs e)
        {
            int SelectedGameId = Convert.ToInt32((sender as StackPanel).Tag);
            Game game = new Game(this, SelectedGameId, userlogin);
            game.Show();
            this.Hide();
        }
        private void Login_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow autorization = new MainWindow();
            autorization.Show();
            this.Close();
        }
        private void Account_Click(object sender, RoutedEventArgs e)
        {
            Account account = new Account(userlogin, this);
            account.Show();
            this.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }
        private void SettingBar_MouseLeave(object sender, MouseEventArgs e)
        {
            SettingBar.Visibility = Visibility.Hidden;
            SearchBox.Visibility=Visibility.Visible;
        }

        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SettingBar.Visibility = Visibility.Visible;
            SearchBox.Visibility=Visibility.Hidden;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchBox.Text!="")
            {
                listGames("Select * from games where name like '%"+SearchBox.Text+"%'");
            }
            else
            {
                listGames("Select * from games");
            }    
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            selectedGenre.Content = "Все игры";
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string genre = (sender as Label).Content.ToString();
            string cmd;
            selectedGenre.Content = genre;
            if (genre == "Очистить")
            {
                cmd = "Select * from games";
                selectedGenre.Content = "Все игры";
            }
            else
                cmd = "Select * from games where genre='" + genre + "'";
            listGames(cmd);
        }

        private void FavouritesPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            selectedGenre.Content = "Избранные";
            listGames("select * from games inner join favourites on games.id=favourites.game_id where favourites.user_id="+connection.select(connection.cmd("Select * from users where login='" + userlogin + "'"), "id"));
        }
    }
}
