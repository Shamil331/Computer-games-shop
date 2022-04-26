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
using System.Drawing;
using System.Drawing.Imaging;
using Image = System.Drawing.Image;
using Microsoft.Win32;
using System.Globalization;
using System.Data.SqlClient;
using Rectangle = System.Drawing.Rectangle;
using System.Drawing.Drawing2D;

namespace Computer_games_shop
{
    /// <summary>
    /// Логика взаимодействия для RedactGames.xaml
    /// </summary>
    public partial class RedactGames : Window
    {
        Connection connection;
        DataTable games;
        string adminLogin;
        string gameInfo;
        string mode;
        bool pictureChanged=false;
        byte[] pictureSource;
        string gamename;
        string gamedeveloper;
        private void fillGamesList()
        {
            connection = new Connection();
            string cmd = "Select name, developer from games";
            games = connection.cmd(cmd);
            int userscount = games.Rows.Count;
            for (int i = 0; i < userscount; i++)
            {
                gameInfo = connection.select_with_number(games, "name", i) + ":" + connection.select_with_number(games, "developer", i);
                ListBoxItem gameinfo = new ListBoxItem();
                gameinfo.Content = gameInfo;
                gameslist.Items.Add(gameinfo);
            }
        }
        public RedactGames(string Login)
        {
            InitializeComponent();
            adminLogin = Login;
            fillGamesList();
            Enable(false);
        }
        public RedactGames()
        {
            InitializeComponent();
        }
        public void Enable(bool b)
        {
            NameBox.IsEnabled = b;
            PriceBox.IsEnabled = b;
            GenreBox.IsEnabled = b;
            ClientBox.IsEnabled = b;
            PlatformBox.IsEnabled = b;
            DeveloperBox.IsEnabled = b;
            DateBox.IsEnabled = b;
            DescriptionBox.IsEnabled = b;
            RequirementsBox.IsEnabled = b;
            ChooseImageButton.IsEnabled = b;
        }
        private void Game_Shop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AdminMain adminMain = new AdminMain(adminLogin);
            adminMain.Show();
            this.Close();
        }

        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SettingBar.Visibility = Visibility.Visible;
            gameslist.Visibility = Visibility.Hidden;
        }

        private void SettingBar_MouseLeave(object sender, MouseEventArgs e)
        {
            SettingBar.Visibility = Visibility.Hidden;
            gameslist.Visibility= Visibility.Visible;
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            Account account = new Account(adminLogin, this);
            account.Show();
            this.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            pictureChanged = true;
            try
            {
                OpenFileDialog op = new OpenFileDialog();
                op.ShowDialog();
                pictureSource = File.ReadAllBytes(op.FileName);
            }
            catch
                {
                
            }

        }
        public Image byteArrayToImage(byte[] bytesArr)
        {
            using (MemoryStream memstr = new MemoryStream(bytesArr))
            {
                Image img = Image.FromStream(memstr);
                return img;
            }
        }
        public static byte[] GetBytes(string str)
        {
            return str.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(s => Convert.ToByte(s, 16))
                      .ToArray();
        }
        public Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        private void gameslist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string[] gameInfo = gameslist.SelectedItem.ToString().Split(':');
            gamename = gameInfo[1];
            string[] temp = gamename.Split(' ');
            gamename = temp[1];
            gamedeveloper = gameInfo[2];
            string cmd = "Select * from games where name='"+gamename+"' and developer='"+gamedeveloper+"'";
            games = connection.cmd(cmd);
            string gameJenre = connection.select(games, "genre");
            string gameClient = connection.select(games, "client");
            string gamePlatform = connection.select(games, "platform");
            //DateTime gameRelease_Date = Convert.ToDateTime(connection.select(games, "release_date"));
            string gamePrice = connection.select(games, "price");
            string gameRelease_Date_InString = connection.select(games, "release_date");
            string gameDescription=connection.select(games, "description");
            string gameSystem_Requirements = connection.select(games, "system_requirements");
            Image gamePreview;
            BitmapImage imgsource = new BitmapImage();
            MemoryStream memorystream = new MemoryStream();
            try
            {
                byte[] BitImg = connection.img("Select preview from games where name='" + gamename + "'");
                gamePreview = byteArrayToImage(BitImg);
                gamePreview = ResizeImage(gamePreview, (int)previewImage.Width, (int)previewImage.Height);
                gamePreview.Save(memorystream, ImageFormat.Bmp);
                imgsource.BeginInit();
                imgsource.StreamSource = new MemoryStream(memorystream.ToArray());
                imgsource.EndInit();
            }
            catch
            {
                gamePreview = Image.FromFile("C:/Users/79172/Desktop/Магазин компьютерных игр/blago.jpg");
                gamePreview.Save(memorystream, ImageFormat.Bmp);
                imgsource.BeginInit();
                imgsource.StreamSource = new MemoryStream(memorystream.ToArray());
                imgsource.EndInit();
            }
            NameBox.Text = gamename;
            GenreBox.Text = gameJenre;
            ClientBox.Text = gameClient;
            PlatformBox.Text= gamePlatform;
            DeveloperBox.Text = gamedeveloper;
            DateBox.Text = gameRelease_Date_InString;
            PriceBox.Text = gamePrice;
            DescriptionBox.Text = gameDescription;
            RequirementsBox.Text = gameSystem_Requirements;
            previewImage.Source = imgsource;
            Change.IsEnabled = true;
            Add.IsEnabled = false;
            Apply.IsEnabled = false;
            Delete.Visibility = Visibility.Visible;
            Keys.Visibility = Visibility.Visible;
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            Change.IsEnabled = false;
            Add.IsEnabled = false;
            Apply.IsEnabled = true;
            Keys.IsEnabled = false;
            gameslist.IsEnabled=false;
            Delete.Visibility = Visibility.Collapsed;
            mode = "Changing";
            Enable(true);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Change.IsEnabled = false;
            Add.IsEnabled = false;
            Apply.IsEnabled=true;
            Keys.IsEnabled=false;
            gameslist.IsEnabled = false;
            Delete.Visibility=Visibility.Collapsed;
            mode = "Adding";
            Enable(true);
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            string tempName, tempGenre, tempClient, tempPlatform, tempDeveloper, tempDescription, tempRequirements, tempPrice;
            string tempDate;
            byte[] tempPreview;
            tempName = NameBox.Text;
            tempGenre = GenreBox.Text;
            tempClient = ClientBox.Text;
            tempPlatform = PlatformBox.Text;
            tempDeveloper = DeveloperBox.Text;
            tempDescription = DescriptionBox.Text;
            tempRequirements = RequirementsBox.Text;
            tempDate = DateBox.Text;
            tempPrice = PriceBox.Text;
            tempPreview = pictureSource;
            if (mode == "Adding")
            {
                if (NameBox.Text == "" || GenreBox.Text == "" || ClientBox.Text == "" || PlatformBox.Text == "" || DeveloperBox.Text == "" || DateBox.Text == "Выбор даты" || DescriptionBox.Text == "" || RequirementsBox.Text == "" || !pictureChanged)
                {
                    MessageBox.Show("Заполните все поля");
                }
                else
                {
                    string cmd = "Select * from games where name='" + tempName + "'";
                    DataTable check = connection.cmd(cmd);
                    if (check.Rows.Count > 0)
                        MessageBox.Show("Такая игра уже есть в каталоге.");
                    else
                    {
                        cmd = "Insert into games values ('" + tempName + "',"+tempPrice+",'" + tempGenre + "','" + tempClient + "','" + tempPlatform + "','" + tempDeveloper + "','" + tempDate + "',0,'" + tempDescription + "','" + tempRequirements + "',0x" + String.Join("", tempPreview.Select(n => n.ToString("X2"))) + ")";
                        connection.cmd(cmd);
                    }
                }
            }
            if (mode == "Changing")
            {
                if (pictureChanged)
                {
                    if (!(NameBox.Text == "" || GenreBox.Text == "" || ClientBox.Text == "" || PlatformBox.Text == "" || DeveloperBox.Text == "" || DateBox.Text == "Выбор даты" || DescriptionBox.Text == "" || RequirementsBox.Text == ""))
                    {
                        string cmd = "Update games set name= '" + tempName + "', price=" + tempPrice + ", genre='" + tempGenre + "', client='" + tempClient + "', platform='" + tempPlatform + "', developer='" + tempDeveloper + "', release_date='" + tempDate + "', description='" + tempDescription + "', system_requirements='" + tempRequirements + "', preview=0x" + String.Join("", tempPreview.Select(n => n.ToString("X2"))) + " where name='" + gamename + "'";
                        connection.cmd(cmd);
                    }
                }
                if(!pictureChanged)
                {
                    if (!(NameBox.Text == "" || GenreBox.Text == "" || ClientBox.Text == "" || PlatformBox.Text == "" || DeveloperBox.Text == "" || DateBox.Text == "Выбор даты" || DescriptionBox.Text == "" || RequirementsBox.Text == ""))
                    {
                        string cmd = "Update games set name= '" + tempName + "', price=" + tempPrice + ", genre='" + tempGenre + "', client='" + tempClient + "', platform='" + tempPlatform + "', developer='" + tempDeveloper + "', release_date='" + tempDate + "', description='" + tempDescription + "', system_requirements='" + tempRequirements + "' where name='" + gamename + "'";
                        connection.cmd(cmd);
                    }
                }
            }
            Change.IsEnabled = false;
            Add.IsEnabled = true;
            Apply.IsEnabled = false;
            gameslist.IsEnabled = true;
            RedactGames rd = new RedactGames(adminLogin);
            this.Close();
            rd.Show();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            mode = "Deletion";
            Delete.Visibility=Visibility.Collapsed;
            string cmd = "Delete games where name='" + gamename + "' and developer='"+gamedeveloper+"'";
            connection.cmd(cmd);
            RedactGames rd= new RedactGames(adminLogin);
            this.Close();
            rd.Show();
        }

        private void Keys_Click(object sender, RoutedEventArgs e)
        {
            KeyGenerator KG = new KeyGenerator(gamename, gamedeveloper, adminLogin);
            this.Close();
            KG.Show();
            
        }
    }
}
