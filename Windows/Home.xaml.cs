using Blaze.Functions;
using DiscordRPC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Steamworks;

namespace Blaze.Windows
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public bool serverSelected = false;
        public bool searching = false;

        public Home()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate { DragMove(); };

            ServerList.ItemsSource = Variables.ServerList;
            GameList.ItemsSource = Variables.GameList;

            if (Variables.DiscordConnected)
            {
                Functions.Discord.discord.client.ClearPresence();

                //Set status on Discord.
                Functions.Discord.discord.client.SetPresence(new RichPresence()
                {
                    Details = "Browsing Servers...",
                    State = "",
                    Timestamps = Functions.Discord.startTime,
                    Assets = new Assets()
                    {
                        LargeImageKey = "blaze2",
                        LargeImageText = "Devlin.gg/Blaze",
                    }
                });
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void GameList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            SearchingServersTXT.Visibility = Visibility.Visible;
            searching = true;
            ServerList.ItemsSource = null;
            UpdateServers();
        }

        private void ServerList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            serverSelected = true;
        }

        private async Task UpdateServers()
        {
            Variables.CurrGame = Variables.GameList[GameList.SelectedIndex];
            await Functions.Servers.GetServers();
            
            ServerList.ItemsSource = Variables.ServerList;
            searching = false;
            SearchingServersTXT.Visibility = Visibility.Hidden;
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (serverSelected && !searching)
            {
                GameRunning.Visibility = Visibility.Visible;
                Server currServer = Variables.ServerList[ServerList.SelectedIndex];

                if (Variables.DiscordConnected)
                {
                    Functions.Discord.discord.client.ClearPresence();

                    //Set status on Discord.
                    Functions.Discord.discord.client.SetPresence(new RichPresence()
                    {
                        Details = currServer.ServerName,
                        State = "Map: " + currServer.Map + " | Players: " + currServer.CurrPlayers + "/" + currServer.MaxPlayers,
                        Timestamps = Functions.Discord.startTime,
                        Assets = new Assets()
                        {
                            LargeImageKey = Variables.CurrGame.AppID.ToString(),
                            LargeImageText = Variables.CurrGame.Title,
                        }
                    });
                }

                SteamClient.Init(Variables.CurrGame.AppID);

                Process game = new Process();
                game.StartInfo.FileName = SteamApps.AppInstallDir(currServer.Game.AppID) + "\\" + Variables.CurrGame.Filename;
                game.StartInfo.Arguments = "-connect=" + currServer.IPandPort;
                
                SteamClient.Shutdown();
                game.Start();
                game.WaitForExit();
                if (Variables.DiscordConnected)
                {
                    Functions.Discord.discord.client.ClearPresence();

                    //Set status on Discord.
                    Functions.Discord.discord.client.SetPresence(new RichPresence()
                    {
                        Details = "Browsing Servers...",
                        State = "",
                        Timestamps = Functions.Discord.startTime,
                        Assets = new Assets()
                        {
                            LargeImageKey = "blaze2",
                            LargeImageText = "Devlin.gg/Blaze",
                        }
                    });
                }
                GameRunning.Visibility = Visibility.Hidden;
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Functions.Discord.discord.client.Deinitialize();
            Environment.Exit(0);
        }
    }
}
