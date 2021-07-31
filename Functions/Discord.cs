using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Blaze.Functions
{
    class Discord
    {
        public static Discord discord = new Discord();

        public DiscordRpcClient client;

        public static Windows.Home home;

        public static Timestamps startTime = Timestamps.Now;

        public Discord()
        {
            client = new DiscordRpcClient("867039623737770053", -1, autoEvents: true, client: new DiscordRPC.IO.ManagedNamedPipeClient());
            client.SetSubscription(EventType.Join | EventType.JoinRequest);
            client.RegisterUriScheme();
            client.OnJoin += OnJoin;

            client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };
            client.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence);
            };

            client.Initialize();
        }

        public static void JoinServer()
        {
            string tmpsecret = "Murderous Pursuits,90149646132336641";
            //string[] secret = args.Secret.Split(',');
            string[] secret = tmpsecret.Split(',');
            var filteredGames = Variables.GameList.Where(game => game.Title == secret[0]).ToList();
            if (filteredGames.Any())
            {
                var filteredServer = Variables.ServerList.Where(server => server.SteamID == secret[1]).ToList();
                home.JoinServer(filteredServer.First(), true);
            }
        }

        private static void OnJoin(object sender, JoinMessage args)
        {
            string[] secret = args.Secret.Split(',');
            var filteredGames = Variables.GameList.Where(game => game.AppID.ToString() == secret[0]).ToList();
            if (filteredGames.Any())
            {
                var filteredServer = Variables.ServerList.Where(server => server.SteamID == secret[1]).ToList();
                if (filteredServer.Any()) home.JoinServer(filteredServer.First(), true);
                else MessageBox.Show("Cant find server.");


            } else MessageBox.Show("It looks like you dont have that game :/");
        }


        public void Deinitialize()
        {
            client.ClearPresence();
            client.Dispose();
        }
    }
}
