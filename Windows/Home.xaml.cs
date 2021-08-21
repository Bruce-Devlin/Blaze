using Blaze.Functions;
using DiscordRPC;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            AddGame.home = this;
            Functions.Blaze.home = this;

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
                Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "Download Blaze!", Url = "https://devlin.gg/Blaze" } },
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
            await Functions.Blaze.Say("Hello! My name is Blaze, I can help you join and host servers for your favorite Blazing Griffin games. You can even add your own Steam games and I can try working for that game too!", "Happy");

            GameList.ItemsSource = Variables.GameList;
            await UpdateGames();
            await Functions.Servers.GetExtIP();
        }

        public async Task UpdateGames() { GameList.Items.Refresh(); }

        private async void UpdateCurrGame()
        {
            if (!searchingForServers)
            {
                if (!Variables.CurrGame.Running)
                {
                    Variables.CurrGame = (Game)GameList.SelectedItem;

                    Variables.CurrGame.Running = false;
                    //Set status on Discord.
                    Functions.Discord.discord.client.ClearPresence();
                    Functions.Discord.discord.client.SetPresence(new RichPresence()
                    {
                        Details = "Browsing Servers...",
                        State = "(" + Variables.CurrGame.Title + ")",
                        Timestamps = Functions.Discord.startTime,
                        Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "Download Blaze!", Url = "https://devlin.gg/Blaze" } },
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
                    await Functions.Blaze.Say("It looks like you already have a game open, close it and then try switching game again.", "Really");
                }

            }
            else { GameList.UnselectAll(); }
        }

        private async void GameList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            UpdateCurrGame();
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

        private async void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ServerList.SelectedItem != null)
            {
                await Functions.Blaze.Say("I'll just wait here until you have closed the game!", "Happy");
                JoinServer((Server)ServerList.SelectedItem);
            }
            else await Functions.Blaze.Say("You have to select a server first.", "Wait");
        }

        public async void JoinServer(Functions.Server currServer, bool joining = false)
        {
            if (joining || serverSelected && !searchingForServers)
            {
                if (await Games.IsGameRunning()) await Functions.Blaze.Say("Game already running, try closing it and trying again.", "Really");
                else
                {
                    
                    //Set status on Discord.
                    Functions.Discord.discord.client.ClearPresence();
                    List<int> party = new List<int>() { currServer.Info.players + 1, currServer.Info.max_players }; ; 

                    if (party[0] > party[1]) party[1] = 99;

                    string imagekey = "nutural";
                    if (Variables.CurrGame.BlazingGriffin) imagekey = Variables.CurrGame.AppID.ToString();
                    Functions.Discord.discord.client.SetPresence(new RichPresence()
                    {
                        Details = currServer.Info.name,
                        State = "Map: " + currServer.Info.map + " | Players: ",
                        Timestamps = Functions.Discord.startTime,
                        //Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button(){ Label = "Download Blaze!", Url = "https://devlin.gg/Blaze"}},
                        Party = new Party()
                        {
                            ID = currServer.Info.name,
                            Size = party[0],
                            Max = party[1],
                            Privacy = Party.PrivacySetting.Public
                        },
                        Secrets = new Secrets()
                        {
                            JoinSecret = currServer.Game.AppID + "," + currServer.SteamID
                        },
                        Assets = new Assets()
                        {
                            LargeImageKey = imagekey.ToString(),
                            LargeImageText = Variables.CurrGame.Title,
                        }
                    });
                    try
                    {
                        SteamClient.Init(Variables.CurrGame.AppID);
                        Process game = new Process();
                        game.StartInfo.FileName = SteamApps.AppInstallDir(currServer.Game.AppID) + "\\" + Variables.CurrGame.Filename;
                        SteamClient.Shutdown();


                        game.StartInfo.Arguments = "-connect=" + currServer.IpandPort();
                        game.Start();
                        game.WaitForExit();

                        //Set status on Discord.
                        Functions.Discord.discord.client.ClearPresence();
                        Functions.Discord.discord.client.SetPresence(new RichPresence()
                        {
                            Details = "Browsing Servers...",
                            State = "(" + Variables.CurrGame.Title + ")",
                            Timestamps = Functions.Discord.startTime,
                            Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "Download Blaze!", Url = "https://devlin.gg/Blaze" } },
                            Assets = new Assets()
                            {
                                LargeImageKey = "nutural",
                                LargeImageText = "Devlin.gg/Blaze",
                            }
                        });
                    }
                    catch (Exception Ex)
                    {
                        await Functions.Blaze.Say(Ex.Message.ToString(), "Sleepy");
                    }
                }
            }
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
                Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "Download Blaze!", Url = "https://devlin.gg/Blaze" } },
                Assets = new Assets()
                {
                    LargeImageKey = "nutural",
                    LargeImageText = "Devlin.gg/Blaze",
                }
            });
        }

        private async void AddGameBtn_Click(object sender, RoutedEventArgs e)
        {

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
            
        }

        private async void RemoveGameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Variables.CurrGame.BlazingGriffin) await Functions.Blaze.Say("Blazing Griffin games can't be removed from the launcher.", "Sad");
            else
            {
                
                Variables.GameList.Remove(Variables.GameList[GameList.SelectedIndex]);
                await Functions.Games.SetLocalGames();
                GameList.SelectedIndex = 0;
                UpdateGames();
                
                UpdateCurrGame();
            }
        }

        private async void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            UpdateServers();
        }


        private void SearchBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) { SearchBox.Text = ""; }

        private async void MyServersBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Variables.CurrGame.AppID == 383790) await Functions.Blaze.Say("Sorry, I can't host The Ship Servers yet... Maybe try the Murderous Pursuits as that works; sorta?", "Sad");
            else if (Variables.CurrGame.BlazingGriffin)
            {
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
                    Buttons = new DiscordRPC.Button[] { new DiscordRPC.Button() { Label = "Download Blaze!", Url = "https://www.devlin.gg/Blaze" } },
                    Assets = new Assets()
                    {
                        LargeImageKey = "nutural",
                        LargeImageText = "Devlin.gg/Blaze",
                    }
                });
            }
            else await Functions.Blaze.Say("Sorry, you cant host a server for a custom using a me; I'm advanced but just not quite \"that\" advanced.", "Sad");
        }

        private async void BlazeCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            await Functions.Blaze.Say();
            BlazeCloseBtn.Visibility = Visibility.Hidden;
            BlazeTxtBox.Visibility = Visibility.Hidden;
            BlazeTxtBG.Visibility = Visibility.Hidden;
        }

        private void DiscordBtn_Click(object sender, RoutedEventArgs e) { System.Diagnostics.Process.Start("https://discord.com/invite/blazinggriffin"); }
        private async void Blaze_Click(object sender, RoutedEventArgs e) 
        {
            await Functions.Blaze.Poke();
            await Functions.Blaze.Say();
        }

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
