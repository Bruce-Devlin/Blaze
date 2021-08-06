using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        public string SteamID { get; set; }
        public bool Mine { get; set; }
    }

    class Servers
    {
        static List<string> servers = new List<string>();

        public static async Task GetServers()
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

                    string[] newIPandPort = newServer.IPandPort.Split(':');
                    if (address == newIPandPort[0]) Variables.MyServers.Add(newServer);
                    else Variables.ServerList.Add(newServer);
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


        public static async Task CreateServer()
        {
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
