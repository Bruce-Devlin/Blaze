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
        public ImageBrush Img { get; set; }
        public string Title { get; set; }
        public uint AppID { get; set; }
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

                    newGame.Title = GameInfo[0];
                    newGame.AppID = uint.Parse(GameInfo[1]);
                    // Get the stream for the image
                    WebRequest request = WebRequest.Create(GameInfo[2]);
                    WebResponse response = request.GetResponse();
                    Stream s = response.GetResponseStream();

                    // Load the stream into the image
                    BitmapImage image = new BitmapImage();
                    image.StreamSource = s;

                    // Apply image as source
                    ImageBrush imgBrush = new ImageBrush();
                    imgBrush.ImageSource = image;

                    newGame.Img = imgBrush;
                    newGame.LinkURL = GameInfo[3];
                    newGame.Filename = GameInfo[4];

                    Variables.GameList.Add(newGame);
                }
            }

            reader.Close();
        }
    }
}
