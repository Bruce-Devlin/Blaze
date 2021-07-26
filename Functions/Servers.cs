using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using Steamworks;

namespace Blaze.Functions
{
    public class Server
    {
        public string ServerName { get; set; }
        public string Map { get; set; }
        public string TotalPlayers { get; set; }
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public string IPandPort { get; set; }
        public Game Game { get; set; }
    }

    class Servers
    {
        static List<string> servers = new List<string>();

        public static async Task GetServers()
        {
            Variables.ServerList.Clear();
            try
            {
                JObject result;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://devlin.gg/blaze/api/ServerList?appID=" + Variables.CurrGame.AppID);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = JObject.Parse(await reader.ReadToEndAsync());

                }
                foreach (JObject server in result["data"]) 
                { 
                    Server newServer = server.ToObject<Server>();
                    newServer.TotalPlayers = server.ToObject<Server>().CurrentPlayers + "/" + server.ToObject<Server>().MaxPlayers;
                    newServer.Game = Variables.CurrGame;
                    Variables.ServerList.Add(newServer); 
                };

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }

        public static async Task GetServerList()
        {
            Variables.ServerList = new List<Server>();
            using (var list = new Steamworks.ServerList.Internet())
            {   
                list.AppId = Variables.CurrGame.AppID;
                await list.RunQueryAsync();

                
            }

            SteamClient.Shutdown();
        }
    }
}
