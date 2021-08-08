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
            Windows.AddGame.home = this;

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
                    LargeImageKey = "nutural",
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
            Functions.Blaze.Say(this, "Hello! My name is Blaze, I can help you join and host servers for your favorite Blazing Griffin games. You can even add your own Steam games and I can try working for that game too!", "Happy");

            await UpdateGames();
            List<int> Ports = new List<int>() { 7776, 7777, 7778 };
            await Functions.Servers.CheckPorts(Ports);
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
                            LargeImageKey = "nutural",
                            LargeImageText = "Devlin.gg/Blaze",
                        }
                    });

                    Window.Background = Variables.CurrGame.Background;

                    StatusBox.Text = "Searching for servers...";
                    UpdateServers();
                }
                else
                {
                    Functions.Blaze.Say(this, "It looks like you already have a game open, close it and then try switching game again.", "Really");
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
            else Functions.Blaze.Say(this, "You have to select a server first.", "Wait");
        }

        public async void JoinServer(Functions.Server currServer, bool joining = false)
        {
            if (joining || serverSelected && !searchingForServers)
            {
                if (await Games.IsGameRunning()) Functions.Blaze.Say(this, "Game already running, try closing it and trying again.", "Really");
                else
                {
                    //Set status on Discord.
                    Functions.Discord.discord.client.ClearPresence();
                    Functions.Discord.discord.client.SetPresence(new RichPresence()
                    {
                        Details = currServer.Info.name,
                        State = "Map: " + currServer.Info.map + " | Players: ",
                        Timestamps = Functions.Discord.startTime,
                        Party = new Party()
                        {
                            ID = currServer.Info.name,
                            Size = currServer.Info.players + 1,
                            Max = currServer.Info.max_players,
                            Privacy = Party.PrivacySetting.Public
                        },
                        Secrets = new Secrets()
                        {
                            JoinSecret = currServer.Game.AppID + "," + currServer.SteamID
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
                       

                        game.StartInfo.Arguments = "-connect=" + currServer.Info.addr + ":" + currServer.Info.gameport;
                        game.Exited += Game_Exited;
                        WindowFade.Visibility = Visibility.Visible;
                        game.Start();
                        game.WaitForExit();
                        WindowFade.Visibility = Visibility.Hidden;

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
                                LargeImageKey = "nutural",
                                LargeImageText = "Devlin.gg/Blaze",
                            }
                        });
                    }
                    catch (Exception Ex)
                    {
                        Functions.Blaze.Say(this, Ex.Message.ToString(), "Sleepy");
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

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowFade.Visibility = Visibility.Visible;
            Windows.Settings settingsWin = new Windows.Settings(this);
            settingsWin.Owner = this;
            settingsWin.ShowDialog();
            WindowFade.Visibility = Visibility.Hidden;
            Functions.Discord.discord.client.ClearPresence();
            Functions.Discord.discord.client.SetPresence(new RichPresence()
            {
                Details = "Browsing Servers...",
                State = "(" + Variables.CurrGame.Title + ")",
                Timestamps = Functions.Discord.startTime,
                Assets = new Assets()
                {
                    LargeImageKey = "nutural",
                    LargeImageText = "Devlin.gg/Blaze",
                }
            });
        }

        private async void AddGameBtn_Click(object sender, RoutedEventArgs e)
        {
            Functions.Blaze.Say(this, "Woah there, this bit is still being worked on and is closed for now. Keep your eyes out for new updates as I should release this soon!", "Happy");
            /*
            WindowFade.Visibility = Visibility.Visible;
            Windows.AddGame addGameWin = new Windows.AddGame(this);
            addGameWin.Owner = this;
            addGameWin.ShowDialog();
            WindowFade.Visibility = Visibility.Hidden;
            await UpdateGames();
            Functions.Discord.discord.client.ClearPresence();
            Functions.Discord.discord.client.SetPresence(new RichPresence()
            {
                Details = "Browsing Servers...",
                State = "(" + Variables.CurrGame.Title + ")",
                Timestamps = Functions.Discord.startTime,
                Assets = new Assets()
                {
                    LargeImageKey = "nutural",
                    LargeImageText = "Devlin.gg/Blaze",
                }
            });
            */
        }

        private async void RemoveGameBtn_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (!Variables.GameList[GameList.SelectedIndex].BlazingGriffin)
            {
                Variables.GameList.RemoveAt(GameList.SelectedIndex);
                await Functions.Games.RemoveGames_TMP();
                UpdateGames();
            }
            */
        }

        private async void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            UpdateServers();
        }


        private void SearchBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) { SearchBox.Text = ""; }

        private async void AddServerBtn_Click(object sender, RoutedEventArgs e)
        {
            Functions.Blaze.Say(this, "Woah there, this bit is still being worked on and is closed for now. Keep your eyes out for new updates as I should release this soon!", "Happy");

            /*
            WindowFade.Visibility = Visibility.Visible;
            Windows.MyServers server = new Windows.MyServers();
            server.Owner = this;
            server.ShowDialog();
            WindowFade.Visibility = Visibility.Hidden;
            Functions.Discord.discord.client.ClearPresence();
            Functions.Discord.discord.client.SetPresence(new RichPresence()
            {
                Details = "Browsing Servers...",
                State = "(" + Variables.CurrGame.Title + ")",
                Timestamps = Functions.Discord.startTime,
                Assets = new Assets()
                {
                    LargeImageKey = "nutural",
                    LargeImageText = "Devlin.gg/Blaze",
                }
            });
            */
        }

        private async void BlazeCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Functions.Blaze.Say(this, "", "Nutural");
            BlazeCloseBtn.Visibility = Visibility.Hidden;
            BlazeTxtBox.Visibility = Visibility.Hidden;
            BlazeTxtBG.Visibility = Visibility.Hidden;
        }

        private void DiscordBtn_Click(object sender, RoutedEventArgs e) { System.Diagnostics.Process.Start("https://discord.com/invite/blazinggriffin"); }
        private void Blaze_Click(object sender, RoutedEventArgs e) { System.Diagnostics.Process.Start("https://devlin.gg/blaze"); }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (SearchBox.Text != "")
            {
                var filtered = Variables.ServerList.Where(server => server.Info.name.ToLower().Contains(SearchBox.Text.ToLower()));
                ServerList.ItemsSource = filtered;
            }
            else ServerList.ItemsSource = Variables.ServerList;
        }

    }
}
