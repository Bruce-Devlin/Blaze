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

            Window.Background = Variables.GameList[0].Background;

            //Set status on Discord.
            Functions.Discord.discord.client.ClearPresence();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void GameList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (!searching)
            {
                Variables.CurrGame = Variables.GameList[GameList.SelectedIndex];

                Window.Background = Variables.CurrGame.Background;

                SearchingServersTXT.Visibility = Visibility.Visible;
                searching = true;
                ServerList.ItemsSource = null;
                UpdateServers();
            } else { GameList.UnselectAll(); }
        }

        private void ServerList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            serverSelected = true;
        }

        private async Task UpdateServers()
        {
            
            await Functions.Servers.GetServers();
            
            ServerList.ItemsSource = Variables.ServerList;
            searching = false;
            SearchingServersTXT.Visibility = Visibility.Hidden;
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            JoinServer(Variables.ServerList[ServerList.SelectedIndex]);
        }

        public async void JoinServer(Server currServer)
        {
            if (serverSelected && !searching)
            {
                if (await Games.IsGameRunning()) MessageBox.Show("Game already running, try closing it and trying again.");
                else
                {
                    //Set status on Discord.
                    Functions.Discord.discord.client.ClearPresence();

                    Functions.Discord.discord.client.SetPresence(new RichPresence()
                    {
                        Details = currServer.ServerName,
                        State = "Map: " + currServer.Map + " | Players: " + (currServer.CurrentPlayers + 1) + "/" + currServer.MaxPlayers,
                        Timestamps = Functions.Discord.startTime,
                        Party = new Party()
                        {
                            ID = currServer.ServerName,
                            Size = currServer.CurrentPlayers + 1,
                            Max = currServer.MaxPlayers,
                            Privacy = Party.PrivacySetting.Public
                        },
                        Secrets = new Secrets()
                        {
                            JoinSecret = currServer.IPandPort
                        },
                        Assets = new Assets()
                        {
                            LargeImageKey = Variables.CurrGame.AppID.ToString(),
                            LargeImageText = Variables.CurrGame.Title,
                        }
                    });

                    SteamClient.Init(Variables.CurrGame.AppID);

                    Process game = new Process();
                    game.StartInfo.FileName = SteamApps.AppInstallDir(currServer.Game.AppID) + "\\" + Variables.CurrGame.Filename;
                    game.StartInfo.Arguments = "-connect=" + currServer.IPandPort;

                    SteamClient.Shutdown();
                    game.Start();
                }
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Functions.Discord.discord.Deinitialize();
            Environment.Exit(0);
        }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void AddGameBtn_Click(object sender, RoutedEventArgs e)
        {
            Windows.AddGame addGameWin = new Windows.AddGame();
            addGameWin.Owner = this;
            addGameWin.ShowDialog();
            GameList.ItemsSource = Variables.GameList;
        }
    }
}
