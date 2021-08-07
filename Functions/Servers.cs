using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
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
        public string SteamID { get; set; }
    }

    public class MyServer
    {
        public string ServerName { get; set; }
        public uint AppID { get; set; }
        public string Filename { get; set; }
    }

    public class MP_ServerConfig
    {
        public int serverType { get; set; }
        public int serverPort { get; set; }
        public string serverName { get; set; }
        public string password { get; set; }
        public int lobbyCountdownSecs { get; set; }
        public int map { get; set; }
        public bool mapCycle { get; set; }
        public int numberOfBots { get; set; }
        public int gameMode { get; set; }
        public int gameEndCondition { get; set; }
        public int winningScore { get; set; }
        public int roundCount { get; set; }
        public int roundTimeLimit { get; set; }
        public int soleSurvivorBonus { get; set; }
        public int gameCountdownMins { get; set; }
        public int weaponReward1st { get; set; }
        public int weaponReward2nd { get; set; }
        public int weaponReward3rd { get; set; }
        public int weaponReward4th { get; set; }
        public int weaponReward5th { get; set; }
        public int hunterKillBonus { get; set; }
        public int killsForKillStreak { get; set; }
        public int killStreakBonus { get; set; }
        public int hiddenKillBonus { get; set; }
        public int exposedKillBonus { get; set; }
        public int abilityKillBonus { get; set; }
        public int backstabKillBonus { get; set; }
    }

    class Servers
    {
        static List<string> servers = new List<string>();

        public static async Task GetServers()
        {
            try
            {
                string address = "";
                WebRequest ipRequest = WebRequest.Create("http://checkip.dyndns.org/");
                using (WebResponse response = ipRequest.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    address = stream.ReadToEnd();
                }
                int first = address.IndexOf("Address: ") + 9;
                int last = address.LastIndexOf("</body>");
                address = address.Substring(first, last - first);

                Variables.ServerList.Clear();
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Oh no! I ran into a problem connecting to the mothership? - " + Ex.ToString());
            }
            
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

        public static void StartServer(MyServer server)
        {
            //if (CheckPorts)


            SteamClient.Init(server.AppID);
            var serverDir = SteamApps.AppInstallDir(server.AppID);
            SteamClient.Shutdown();
            
            Process game = new Process();
            game.StartInfo.FileName = serverDir + "\\" + server.Filename;
            

            game.StartInfo.Arguments = "-serverid \"" + server.ServerName + "\"";
            game.Start();
            game.WaitForExit();
        }

        public static async Task UpdateLocalServers()
        {
            if (File.Exists(Variables.ConfigDir + @"\myservers.json"))
            {
                string json = File.ReadAllText(Variables.ConfigDir + @"\myservers.json");
                if (json != "") Variables.LocalServers = JsonConvert.DeserializeObject<List<MyServer>>(json);
            }
        }

        public static async Task SaveLocalServers()
        {
            if (!File.Exists(Variables.ConfigDir + @"\myservers.json")) File.Create(Variables.ConfigDir + @"\myservers.json").Close();
            File.WriteAllText(Variables.ConfigDir + @"\myservers.json", JsonConvert.SerializeObject(Variables.LocalServers));
        }

        public static async Task CreateServer(MyServer server, Windows.MyServers owner)
        {
            try
            {
                SteamClient.Init(server.AppID);
                var serverDir = SteamApps.AppInstallDir(server.AppID);
                SteamClient.Shutdown();

                Directory.CreateDirectory(serverDir + @"\" + server.ServerName);
                if (server.AppID == 689780 && !File.Exists(serverDir + @"\" + server.ServerName + "DedicatedServerConfig.json"))
                {
                    Directory.CreateDirectory(serverDir + @"\" + server.ServerName);
                    WebClient Client = new WebClient();
                    Client.DownloadFile("https://devlin.gg/blaze/mp-config/DedicatedServerConfig.json", serverDir + @"\" + server.ServerName + @"\DedicatedServerConfig.json");
                }

                Variables.LocalServers.Add(server);
                await SaveLocalServers();
                owner.MyServerList.ItemsSource = Variables.LocalServers;

            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }

        public static async Task CheckPorts(List<int> Ports)
        {
            foreach(int port in Ports)
            {
                UdpClient udpClient = new UdpClient();
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));

                var data = Encoding.UTF8.GetBytes("ping");

                udpClient.Send(data, data.Length, "77.72.1.22", port);

                var from = new IPEndPoint(0, 0);
                await Task.Run(() =>
                {
                    while (true)
                    {
                        var recvBuffer = udpClient.Receive(ref from);
                        MessageBox.Show(Encoding.UTF8.GetString(recvBuffer));
                    }
                });

                /*
                if (udpClient.Client.) MessageBox.Show("Port " + port + " is open");
                else MessageBox.Show("Port " + port + " is Closed.");
                //if (System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners().Any(p => p.Port == port)) 
                */
            }
        }
    }
}
