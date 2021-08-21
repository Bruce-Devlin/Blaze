using System;
using System.Collections.Generic;
using System.IO;
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
                Details = "Checking their servers...",
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
            if (MyServerList.SelectedIndex != -1)
            {
                ServerNameBox.Text = Variables.LocalServers[MyServerList.SelectedIndex].ServerName;
                ServerPortBox.Text = Variables.LocalServers[MyServerList.SelectedIndex].ServerConfig.serverPort.ToString();
                ServerPasswordBox.Text = Variables.LocalServers[MyServerList.SelectedIndex].ServerConfig.password;
                if (Variables.LocalServers[MyServerList.SelectedIndex].ServerConfig.password == "")
                {
                    Passworded.IsChecked = false;
                    ServerPasswordBox.IsEnabled = false;
                }
                else
                {
                    Passworded.IsChecked = true;
                }
                
            }
            
        }

        private async void NewServerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ServerNameBox.Text != "")
            {
                //if (Directory.Exists(Variables.CurrGame.))
            }

            Functions.MyServer newServer = new Functions.MyServer();

            newServer.ServerName = ServerNameBox.Text;
            newServer.AppID = Variables.CurrGame.ServerAppID;
            newServer.Filename = Variables.CurrGame.ServerFilename;
            newServer.ServerConfig = new Functions.MP_ServerConfig();
            if (ServerPortBox.Text != "")
            {
                newServer.ServerConfig.serverPort = int.Parse(ServerPortBox.Text);
                newServer.ServerConfig.password = ServerPasswordBox.Text;

                await Functions.Servers.CreateServer(ServerNameBox.Text, ServerPortBox.Text, this);
            }
            else MessageBox.Show("Looks like you forgot to enter a port?");
            
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
            if (MyServerList.SelectedIndex != -1)
            {
                Variables.LocalServers[MyServerList.SelectedIndex].ServerName = ServerNameBox.Text;

                Variables.LocalServers[MyServerList.SelectedIndex].ServerConfig.serverName = ServerNameBox.Text;
                Variables.LocalServers[MyServerList.SelectedIndex].ServerConfig.serverPort = int.Parse(ServerPortBox.Text);
                Variables.LocalServers[MyServerList.SelectedIndex].ServerConfig.password = ServerPasswordBox.Text;

                await Functions.Servers.SetConfig(Variables.LocalServers[MyServerList.SelectedIndex], Variables.LocalServers[MyServerList.SelectedIndex].ServerConfig);

                MyServerList.ItemsSource = new List<Functions.MyServer>();
                MyServerList.ItemsSource = Variables.LocalServers;
                await Functions.Servers.SetLocalServers();
            }
            
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Passworded_Checked(object sender, RoutedEventArgs e)
        {
            ServerPasswordBox.IsEnabled = true;
        }

        private void Passworded_Unchecked(object sender, RoutedEventArgs e)
        {
            ServerPasswordBox.Text = "";
            ServerPasswordBox.IsEnabled = false;
        }
    }
}
