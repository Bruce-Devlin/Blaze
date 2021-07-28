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
        public bool searchingForServers = false;
        public bool searching = false;

        public Home()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate { DragMove(); };
            Discord.home = this;

            var domain = AppDomain.CurrentDomain;

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ProcessExit);
            domain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledEx);

            ServerList.ItemsSource = Variables.ServerList;

            Window.Background = Variables.GameList[0].Background;

            //Set status on Discord.
            Functions.Discord.discord.client.ClearPresence();
            Functions.Discord.discord.client.SetPresence(new RichPresence()
            {
                Details = "Browsing Servers...",
                State = "(" + Variables.CurrGame.Title + ")",
                Timestamps = Functions.Discord.startTime,
                Assets = new Assets()
                {
                    LargeImageKey = "blaze2",
                    LargeImageText = "Devlin.gg/Blaze",
                }
            });
        }

        static void UnhandledEx(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show(e.ToString());
        }

        static void ProcessExit(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        static async Task Shutdown() { Environment.Exit(0); }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateGames();
        }

        public async Task UpdateGames()
        {
            GameList.ItemsSource = Variables.GameList;
        }

        private void GameList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (!searchingForServers)
            {
                if (!Variables.CurrGame.Running)
                {
                    Variables.CurrGame = Variables.GameList[GameList.SelectedIndex];

                    Variables.CurrGame.Running = false;
                    //Set status on Discord.
                    Functions.Discord.discord.client.ClearPresence();
                    Functions.Discord.discord.client.SetPresence(new RichPresence()
                    {
                        Details = "Browsing Servers...",
                        State = "(" + Variables.CurrGame.Title + ")",
                        Timestamps = Functions.Discord.startTime,
                        Assets = new Assets()
                        {
                            LargeImageKey = "blaze2",
                            LargeImageText = "Devlin.gg/Blaze",
                        }
                    });

                    Window.Background = Variables.CurrGame.Background;

                    StatusBox.Text = "Searching for servers...";
                    UpdateServers();
                }
                else
                {
                    MessageBox.Show("It looks like you already have a game open, close it and then try switching game again.");
                }
                
            } else { GameList.UnselectAll(); }
        }

        private void ServerList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            serverSelected = true;
        }

        private async Task UpdateServers()
        {
            ServerList.ItemsSource = null;
            searchingForServers = true;
            StatusBox.Text = "Searching for servers...";
            await Functions.Servers.GetServers();
            
            ServerList.ItemsSource = Variables.ServerList;
            searchingForServers = false;
            StatusBox.Text = "";
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ServerList.SelectedItem != null) JoinServer(Variables.ServerList[ServerList.SelectedIndex]);
            else MessageBox.Show("You have to select a server first.");
        }

        public async void JoinServer(Server currServer, bool joining = false)
        {
            if (joining || serverSelected && !searchingForServers)
            {
                if (await Games.IsGameRunning()) MessageBox.Show("Game already running, try closing it and trying again.");
                else
                {
                    //Set status on Discord.
                    Functions.Discord.discord.client.ClearPresence();
                    Functions.Discord.discord.client.SetPresence(new RichPresence()
                    {
                        Details = currServer.ServerName,
                        State = "Map: " + currServer.Map + " | Players: ",
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
                            JoinSecret = currServer.Game.Title + "," + currServer.SteamID
                        },
                        Assets = new Assets()
                        {
                            LargeImageKey = Variables.CurrGame.AppID.ToString(),
                            LargeImageText = Variables.CurrGame.Title,
                        }
                    });
                    try
                    {
                        SteamClient.Init(Variables.CurrGame.AppID);
                        Process game = new Process();
                        game.StartInfo.FileName = SteamApps.AppInstallDir(currServer.Game.AppID) + "\\" + Variables.CurrGame.Filename;
                        SteamClient.Shutdown();

                        string[] newIPandPort = currServer.IPandPort.Split(':');
                        game.StartInfo.Arguments = "-connect=" + newIPandPort[0] + ":" + newIPandPort[1];
                        game.Exited += Game_Exited;
                        game.Start();
                        game.WaitForExit();

                        //Variables.CurrGame.Running = true;
                        //Variables.CurrGame.Running = false;
                        //Set status on Discord.
                        Functions.Discord.discord.client.ClearPresence();
                        Functions.Discord.discord.client.SetPresence(new RichPresence()
                        {
                            Details = "Browsing Servers...",
                            State = "(" + Variables.CurrGame.Title + ")",
                            Timestamps = Functions.Discord.startTime,
                            Assets = new Assets()
                            {
                                LargeImageKey = "blaze2",
                                LargeImageText = "Devlin.gg/Blaze",
                            }
                        });
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.ToString());
                    }
                }
            }
        }

        private void Game_Exited(object sender, EventArgs e)
        {
            
        }

        private async void CloseBtn_Click(object sender, RoutedEventArgs e) 
        {
            StatusBox.Text = "Shutting Down...";
            await Task.Run(() => Shutdown());
        }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private async void AddGameBtn_Click(object sender, RoutedEventArgs e)
        {
            Windows.AddGame addGameWin = new Windows.AddGame(this);
            addGameWin.Owner = this;
            addGameWin.ShowDialog();
            await UpdateGames();
        }

        private async void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            searching = false;
            UpdateServers();
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!searching && SearchBox.Text != "")
            {
                searching = true;
                var filtered = Variables.ServerList.Where(server => server.ServerName.ToLower().Contains(SearchBox.Text.ToLower()));
                ServerList.ItemsSource = filtered;
            }
            else
            {

            }
        }

        private void SearchBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) { SearchBox.Text = ""; }
    }
}
