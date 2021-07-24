// Disclaimer time, this is NOT endorsed, supported or created/published by Blazing Griffin.
// This is a little app I built to help join servers on MURDEROUS PURSUITS that looks prettier than the Steam servers <3
//
// This app was made by Bruce Devlin, I'm just a fan of BG. You can check me out at https://devlin.gg if you would like.
// This app is completely free and you can even use any of my code if you would like.


using DiscordRPC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
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


namespace Blaze
{
    /// <summary>
    /// Interaction logic for Preload.xaml
    /// </summary>
    public partial class Preload : Window
    {
        public Preload()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate { DragMove(); };
            StatusBox.Text = "Blaze is starting...";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PreloadManager();
        }

        public async void PreloadManager()
        {
            //Set status on Discord.
            Functions.Discord.discord.client.SetPresence(new RichPresence()
            {
                Details = "Starting Blaze...",
                State = "",
                Timestamps = Functions.Discord.startTime,
                Assets = new Assets()
                {
                    LargeImageKey = "blaze2",
                    LargeImageText = "Devlin.gg/Blaze",
                }
            });

            StatusBox.Text = "Checking games...";
            await Functions.Games.GetGames();
            StatusBox.Text = "Got games!";
            Variables.CurrGame = Variables.GameList[0];

            StatusBox.Text = "Checking servers...";
            try 
            { 
                await Functions.Servers.GetServers();
                StatusBox.Text = "Got servers!";
            }
            catch 
            {
                StatusBox.Text = "Steam not running!";
            }
            PreloadDone();
        }

        public void PreloadDone()
        {
            StatusBox.Text = "Preloading complete!";

            Windows.Home home = new Windows.Home();
            home.Show();
            this.Close();
        }
    }
}
