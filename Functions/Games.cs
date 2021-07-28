using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Steamworks;

namespace Blaze.Functions
{
    public class Game
    {
        public ImageSource GameIcon { get; set; }
        public string Title { get; set; }
        public uint AppID { get; set; }
        public ImageBrush Background { get; set; }
        public string LinkURL { get; set; }
        public string Filename { get; set; }
        public string PlainName { get; set; }
        public bool Running { get; set; }
    }

    class Games
    {
        public static async Task<bool> IsGameRunning()
        {

            if (Process.GetProcessesByName(Variables.CurrGame.PlainName).Length > 0) return true;
            else return false;
        }

        public static async Task AddGame()
        {
            //"{GameIcon},{GameTitle},{AppID},{ImgURL},{LinkURL},{filename.exe},{PlainName}"
        }

        public static async Task GetGames()
        {
            Variables.GameList = new List<Game>();
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://devlin.gg/blaze/games.txt");
            StreamReader reader = new StreamReader(stream);
            string gameslist;
            // Read the file and display it line by line.  
            while ((gameslist = reader.ReadLine()) != null)
            {
                if (!gameslist.StartsWith("//") || !gameslist.StartsWith(""))
                {
                    Game newGame = new Game();
                    List<string> GameInfo = gameslist.Split(',').ToList<string>();

                    WebRequest gameIconRequest = WebRequest.Create(GameInfo[0]);
                    WebResponse gameIconResponse = gameIconRequest.GetResponse();

                    WebRequest backgroundRequest = WebRequest.Create(GameInfo[3]);
                    WebResponse backgroundResponse = backgroundRequest.GetResponse();

                    Stream giS = gameIconResponse.GetResponseStream();
                    Stream bgS = backgroundResponse.GetResponseStream();

                    BitmapImage gameIconImage = new BitmapImage();
                    gameIconImage.BeginInit();
                    gameIconImage.StreamSource = giS;
                    gameIconImage.EndInit();

                    BitmapImage backgroundImage = new BitmapImage();
                    backgroundImage.BeginInit();
                    backgroundImage.StreamSource = bgS;
                    backgroundImage.EndInit();

                    ImageBrush gameIconImgBrush = new ImageBrush();
                    gameIconImgBrush.ImageSource = gameIconImage;

                    ImageBrush backgroundImgBrush = new ImageBrush();
                    backgroundImgBrush.ImageSource = backgroundImage;

                    newGame.GameIcon = gameIconImgBrush.ImageSource;
                    newGame.Title = GameInfo[1];
                    newGame.AppID = uint.Parse(GameInfo[2]);
                    newGame.Background = backgroundImgBrush;
                    newGame.LinkURL = GameInfo[4];
                    newGame.Filename = GameInfo[5];
                    newGame.PlainName = GameInfo[6];
                    newGame.Running = false;

                    Variables.GameList.Add(newGame);
                }
            }
            reader.Close();

            if (File.Exists(Directory.GetCurrentDirectory() + "\\games.txt"))
            {
                StreamReader localReader = new StreamReader(Directory.GetCurrentDirectory() + "\\games.txt");
                string localgameslist;
                while ((localgameslist = localReader.ReadLine()) != null && !localReader.ReadLine().StartsWith("//") && localReader.ReadLine() != "")
                {
                    Game newGame = new Game();
                    List<string> GameInfo = localgameslist.Split(',').ToList<string>();

                    WebRequest gameIconRequest = WebRequest.Create(GameInfo[0]);
                    WebResponse gameIconResponse = gameIconRequest.GetResponse();

                    WebRequest backgroundRequest = WebRequest.Create(GameInfo[3]);
                    WebResponse backgroundResponse = backgroundRequest.GetResponse();

                    Stream giS = gameIconResponse.GetResponseStream();
                    Stream bgS = backgroundResponse.GetResponseStream();

                    BitmapImage gameIconImage = new BitmapImage();
                    gameIconImage.BeginInit();
                    gameIconImage.StreamSource = giS;
                    gameIconImage.EndInit();

                    BitmapImage backgroundImage = new BitmapImage();
                    backgroundImage.BeginInit();
                    backgroundImage.StreamSource = bgS;
                    backgroundImage.EndInit();

                    ImageBrush gameIconImgBrush = new ImageBrush();
                    gameIconImgBrush.ImageSource = gameIconImage;

                    ImageBrush backgroundImgBrush = new ImageBrush();
                    backgroundImgBrush.ImageSource = backgroundImage;

                    newGame.GameIcon = gameIconImgBrush.ImageSource;
                    newGame.Title = GameInfo[1];
                    newGame.AppID = uint.Parse(GameInfo[2]);
                    newGame.Background = backgroundImgBrush;
                    newGame.LinkURL = GameInfo[4];
                    newGame.Filename = GameInfo[5];
                    newGame.PlainName = GameInfo[6];
                    newGame.Running = false;

                    Variables.GameList.Add(newGame);
                }
                localReader.Close();
            }
        }

        private static async Task GetGameWeb()
        {
            
        }
    }
}

