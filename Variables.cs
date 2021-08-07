using Blaze.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blaze
{
    class Variables
    {
        public static bool DiscordConnected = false;
        public static bool FirstRun = true;
        public static bool Updated = false;
        public static bool GameRunning = false;

        public static string AppVersion = "";
        public static string Username = "";

        public static Game CurrGame = new Game();
        public static List<Server> ServerList = new List<Server>();
        public static List<MyServer> LocalServers = new List<MyServer>();

        public static List<Game> GameList = new List<Game>();

        public static string HomeDir = Environment.ExpandEnvironmentVariables("%ProgramW6432%") + @"\Blaze";
        public static string ConfigDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Blaze";
    }
}
