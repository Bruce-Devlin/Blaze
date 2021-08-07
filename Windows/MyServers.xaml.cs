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

            CurrGameTitle.Text = Variables.CurrGame.Title;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Functions.Servers.UpdateLocalServers();
            MyServerList.ItemsSource = Variables.LocalServers.Where(server => server.AppID == Variables.CurrGame.ServerAppID);
        }

        private void MyServerList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            ServerNameBox.Text = Variables.LocalServers[MyServerList.SelectedIndex].ServerName;
        }

        private async void NewServerBtn_Click(object sender, RoutedEventArgs e)
        {
            Functions.MyServer newServer = new Functions.MyServer();

            newServer.ServerName = ServerNameBox.Text;
            newServer.AppID = Variables.CurrGame.ServerAppID;
            newServer.Filename = Variables.CurrGame.ServerFilename;

            await Functions.Servers.CreateServer(newServer, this);
        }

        private void StartServerBtn_Click(object sender, RoutedEventArgs e)
        {
            Functions.Servers.StartServer(Variables.LocalServers[MyServerList.SelectedIndex]);
        }

        private void StopServerBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
