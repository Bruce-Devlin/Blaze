using System;
using System.Collections.Generic;
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
using DiscordRPC;
using Newtonsoft.Json;

namespace Blaze.Windows
{
    /// <summary>
    /// Interaction logic for Server.xaml
    /// </summary>
    public partial class MyServers : Window
    {
        private void CloseBtn_Click(object sender, RoutedEventArgs e) { this.Close(); }

        public MyServers()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate { DragMove(); };

            Functions.Discord.discord.client.ClearPresence();
            Functions.Discord.discord.client.SetPresence(new RichPresence()
            {
                Details = "Creating a server...",
                Timestamps = Functions.Discord.startTime,
                Assets = new Assets()
                {
                    LargeImageKey = "nutural",
                    LargeImageText = "Devlin.gg/Blaze",
                }
            });

            
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Functions.Servers.GetLocalServers();
            CurrGameTitle.Text = Variables.CurrGame.Title;
            if (Variables.LocalServers.Count > 0) MyServerList.ItemsSource = Variables.LocalServers.Where(server => server.AppID == Variables.CurrGame.ServerAppID);
        }

        private void MyServerList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private async void NewServerBtn_Click(object sender, RoutedEventArgs e)
        {
            Functions.MyServer newServer = new Functions.MyServer();

            newServer.ServerName = ServerNameBox.Text;
            newServer.AppID = Variables.CurrGame.ServerAppID;
            newServer.Filename = Variables.CurrGame.ServerFilename;

            await Functions.Servers.CreateServer(ServerNameBox.Text, ServerPortBox.Text, this);
        }

        private async void StartServerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MyServerList.SelectedIndex != -1)
            {
                await Functions.Servers.SetConfig(Variables.LocalServers[MyServerList.SelectedIndex], Variables.LocalServers[MyServerList.SelectedIndex].ServerConfig);
                Functions.Servers.StartServer(Variables.LocalServers[MyServerList.SelectedIndex]);
            }
        }

        private void StopServerBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void SaveServerBtn_Click(object sender, RoutedEventArgs e)
        {
            Functions.MyServer server = Variables.LocalServers[MyServerList.SelectedIndex];

            server.ServerName = ServerNameBox.Text;

            server.ServerConfig.serverName = ServerNameBox.Text;

            Variables.LocalServers[MyServerList.SelectedIndex] = server;

            await Functions.Servers.SetConfig(Variables.LocalServers[MyServerList.SelectedIndex], Variables.LocalServers[MyServerList.SelectedIndex].ServerConfig);

            MyServerList.ItemsSource = new List<Functions.MyServer>();
            MyServerList.ItemsSource = Variables.LocalServers;
        }
    }
}
