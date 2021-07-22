using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Steamworks;

namespace Blaze.Functions
{
    public class Game
    {
        public ImageBrush Banner { get; set; }
        public string Title { get; set; }
        public uint AppID { get; set; }
        public ImageBrush Background { get; set; }
        public string LinkURL { get; set; }
        public string Filename { get; set; }
    }

    class Games
    {
        public static async Task GetGames()
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://devlin.gg/blaze/games.txt");
            StreamReader reader = new StreamReader(stream);
            string gameslist;
            // Read the file and display it line by line.  
            while ((gameslist = reader.ReadLine()) != null)
            {
                if (!gameslist.StartsWith("//"))
                {
                    Game newGame = new Game();
                    List<string> GameInfo = gameslist.Split(',').ToList<string>();

                    WebRequest bannerRequest = WebRequest.Create(GameInfo[0]);
                    WebResponse bannerResponse = bannerRequest.GetResponse();

                    WebRequest backgroundRequest = WebRequest.Create(GameInfo[3]);
                    WebResponse backgroundResponse = backgroundRequest.GetResponse();

                    Stream banS = bannerResponse.GetResponseStream();
                    Stream bgS = backgroundResponse.GetResponseStream();

                    BitmapImage bannerImage = new BitmapImage();
                    bannerImage.BeginInit();
                    bannerImage.StreamSource = banS;
                    bannerImage.EndInit();

                    BitmapImage backgroundImage = new BitmapImage();
                    backgroundImage.BeginInit();
                    backgroundImage.StreamSource = bgS;
                    backgroundImage.EndInit();

                    ImageBrush bannerImgBrush = new ImageBrush();
                    bannerImgBrush.ImageSource = bannerImage;

                    ImageBrush backgroundImgBrush = new ImageBrush();
                    backgroundImgBrush.ImageSource = backgroundImage;

                    newGame.Banner = bannerImgBrush;
                    newGame.Title = GameInfo[1];
                    newGame.AppID = uint.Parse(GameInfo[2]);
                    newGame.Background = backgroundImgBrush;
                    newGame.LinkURL = GameInfo[4];
                    newGame.Filename = GameInfo[5];

                    Variables.GameList.Add(newGame);
                }
            }

            reader.Close();
        }
    }
}
