using DiscordRPC;
using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Functions
{
    class Discord
    {
        public static Discord discord = new Discord();

        public DiscordRpcClient client;

        public static Timestamps startTime = Timestamps.Now;
            
        public Discord()
        {
            client = new DiscordRpcClient("867039623737770053", -1, autoEvents: true, client: new DiscordRPC.IO.ManagedNamedPipeClient());
            client.SetSubscription(EventType.Join | EventType.Spectate | EventType.JoinRequest);
            client.RegisterUriScheme();

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


        public void Deinitialize()
        {
            Variables.DiscordConnected = false;
            client.ClearPresence();
            client.Dispose();
        }
    }
}
