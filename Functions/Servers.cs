using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Steamworks;

namespace Blaze.Functions
{
    public class Server
    {
        public string ServerName { get; set; }
        public string Map { get; set; }
        public int CurrPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public string IPandPort { get; set; }
        public Game Game { get; set; }
    }

    class Servers
    {
        static List<string> servers = new List<string>();

        public static async Task GetServers()
        {
            SteamClient.Init(Variables.CurrGame.AppID);
            await GetServerList();
        }

        public static async Task<bool> GetServerList()
        {
            Variables.ServerList = new List<Server>();
            using (var list = new Steamworks.ServerList.Internet())
            {   
                list.AppId = Variables.CurrGame.AppID;
                await list.RunQueryAsync();

                foreach (var server in list.Responsive)
                {
                    Server newServer = new Server();
                    newServer.IPandPort = server.Address.ToString() + ":" + server.ConnectionPort;
                    newServer.ServerName = server.Name;
                    newServer.Map = server.Map; 
                    newServer.CurrPlayers = server.Players;
                    newServer.MaxPlayers = server.MaxPlayers;
                    newServer.Game = Variables.CurrGame;

                    Variables.ServerList.Add(newServer);
                }
            }
            await Shutdown();
            return true;
        }

        public static async Task Shutdown()
        {
            SteamClient.Shutdown();
        }
    }
}
