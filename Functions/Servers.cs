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
        public string TotalPlayers { get; set; }
        public bool Mine { get; set; }
        public string IpandPort() { return Info.addr + ":" + Info.gameport; }
        public SteamServer Info {get; set;}

        public Game Game { get; set; }
        public string SteamID { get; set; }
    }

    public class SteamServer
    {
        public string addr { get; set; }
        public int gameport { get; set; }
        public ulong steamid { get; set; }
        public string name { get; set; }
        public uint appid { get; set; }
        public string gamedir { get; set; }
        public string version { get; set; }
        public string product { get; set; }
        public int region { get; set; }
        public int players { get; set; }
        public int max_players { get; set; }
        public int bots { get; set; }
        public string map { get; set; }
        public bool secure { get; set; }
        public bool dedicated { get; set; }
        public string os { get; set; }
        public string gametype { get; set; }
    }

    public class MyServer
    {
        public string ServerName { get; set; }
        public uint AppID { get; set; }
        public string Filename { get; set; }
        public string ServerDir { get; set; }
        public string Profile { get; set; }
        public MP_ServerConfig ServerConfig { get; set; }
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
        public static async Task GetServers()
        {
            Variables.ServerList.Clear();

            try
            {
                JObject result;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://devlin.gg/blaze/api/ServerList?appID=" + Variables.CurrGame.AppID);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                string extIP = await GetExtIP();

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = JObject.Parse(await reader.ReadToEndAsync());
                }
                if (int.Parse(result["code"].ToString()) != 400)
                {
                    foreach (JObject server in result["data"])
                    {
                        SteamServer newSteamServer = server.ToObject<SteamServer>();
                        var ipWithoutPort = newSteamServer.addr.Split(':');


                        newSteamServer.addr = ipWithoutPort[0];


                        Server newServer = new Server();

                        if (newSteamServer.addr == extIP)
                        {
                            //My server
                            newSteamServer.name = "🏠 " + newSteamServer.name;
                            newServer.Mine = true;
                        }
                        else
                        {
                            newServer.Mine = false;
                        }

                        newServer.Info = newSteamServer;
                        newServer.Game = Variables.CurrGame;
                        newServer.TotalPlayers = newServer.Info.players + "/" + newServer.Info.max_players;

                        Variables.ServerList.Add(newServer);
                    };
                }
                else MessageBox.Show("Uh-oh!? It looks like I cant connect to steam servers... Maybe Steam is down right now? or maybe check you are connected to the internet and restart me.");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }

        public static async Task<string> GetExtIP()
        {
            JObject result;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://devlin.gg/blaze/api/ReturnIP");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                result = JObject.Parse(await reader.ReadToEndAsync());
                return result["data"].ToString();
            }
        }

        public static void StartServer(MyServer server)
        {
            //if (CheckPorts)
            try
            {
                SteamClient.Init(server.AppID);
                var serverDir = SteamApps.AppInstallDir(server.AppID);
                SteamClient.Shutdown();

                Process game = new Process();
                game.StartInfo.FileName = serverDir + "\\" + server.Filename;


                game.StartInfo.Arguments = "-serverid " + server.Profile;
                game.Start();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }

        }

        public static async Task GetLocalServers()
        {
            Variables.LocalServers = new List<MyServer>();
            if (File.Exists(Variables.ConfigDir + @"\myservers.json"))
            {
                string json = File.ReadAllText(Variables.ConfigDir + @"\myservers.json");
                if (json != "") Variables.LocalServers = JsonConvert.DeserializeObject<List<MyServer>>(json);
            }
        }

        public static async Task SetLocalServers()
        {
            if (!File.Exists(Variables.ConfigDir + @"\myservers.json")) File.Create(Variables.ConfigDir + @"\myservers.json").Close();
            File.WriteAllText(Variables.ConfigDir + @"\myservers.json", JsonConvert.SerializeObject(Variables.LocalServers, Formatting.Indented));
        }

        public static async Task<MP_ServerConfig> GetConfig(MyServer server)
        {
            MP_ServerConfig config = new MP_ServerConfig();
            if (server.AppID == 689780 && File.Exists(server.ServerDir + @"\" + server.Profile + @"\DedicatedServerConfig.json"))
            {
                string json = File.ReadAllText(server.ServerDir + @"\" + server.Profile + @"\DedicatedServerConfig.json");
                return JsonConvert.DeserializeObject<MP_ServerConfig>(json);
            }
            else return new MP_ServerConfig();
        }

        public static async Task SetConfig(MyServer server, MP_ServerConfig config)
        {
            if (server.AppID == 689780 && File.Exists(server.ServerDir + @"\" + server.Profile + @"\DedicatedServerConfig.json"))
            {
                File.WriteAllText(server.ServerDir + @"\" + server.Profile + @"\DedicatedServerConfig.json", JsonConvert.SerializeObject(config, Formatting.Indented));
            }
        }

        public static async Task CreateServer(string ServerName, string ServerPort,  Windows.MyServers owner)
        {
            try
            {
                MyServer server = new MyServer();

                server.ServerName = ServerName;
                server.AppID = Variables.CurrGame.ServerAppID;
                server.Filename = Variables.CurrGame.ServerFilename;
                server.Profile = server.ServerName.Replace(" ", "-").ToLower();

                SteamClient.Init(server.AppID);
                server.ServerDir = SteamApps.AppInstallDir(server.AppID);
                SteamClient.Shutdown();

                if (!Directory.Exists(server.ServerDir + @"\" + server.Profile))
                {
                    Directory.CreateDirectory(server.ServerDir + @"\" + server.Profile);
                    if (server.AppID == 689780 && !File.Exists(server.ServerDir + @"\" + server.Profile + "DedicatedServerConfig.json"))
                    {
                        WebClient Client = new WebClient();
                        Client.DownloadFile("https://devlin.gg/blaze/mp-config/DedicatedServerConfig.json", server.ServerDir + @"\" + server.Profile + @"\DedicatedServerConfig.json");
                    }

                    server.ServerConfig = await GetConfig(server);

                    //Do Config
                    server.ServerConfig.serverName = ServerName;


                    await SetConfig(server, server.ServerConfig);

                    Variables.LocalServers.Add(server);
                    owner.MyServerList.ItemsSource = new List<MyServer>();
                    owner.MyServerList.ItemsSource = Variables.LocalServers;

                    await SetLocalServers();
                }
                else MessageBox.Show("You already have a profile with this name, please choose another.");
                
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
