// Disclaimer time, this is NOT endorsed, supported or created/published by Blazing Griffin and is a fan project.
// This is a little app I built to help join servers on MURDEROUS PURSUITS that looks prettier than the Steam servers <3
//
// This app was made by Bruce Devlin, I'm just a fan of BG. You can check me out at https://devlin.gg if you would like.
// This app is completely free and you can even use any of my code if you would like.


using IWshRuntimeLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Windows;
using System.Deployment;
using System.Reflection;
using System.Threading.Tasks;

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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await PreloadManager();
        }

        internal static void RestartElevated()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
            startInfo.Verb = "runas";
            try
            {
                Process p = Process.Start(startInfo);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                return;
            }
            Environment.Exit(0);
        }

        public bool IsElevated
        {
            get { return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator); }
        }

        public async Task PreloadManager()
        {

            StatusBox.Text = "Making sure you are connected...";
            try { WebRequest testConnection = WebRequest.Create("https://devlin.gg/"); }
            catch
            {
                System.Windows.MessageBox.Show("Uh Oh... It looks like you arnt connected to the internet, check your network and then restart the app");
                Environment.Exit(0);
            }
            try
            {
                StatusBox.Text = "Checking installation...";
                if (!await Functions.Install.CheckInstallation()) { await Functions.Install.StartInstall(this); }
                else
                {
                    if (!Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).EndsWith(@"Blaze") && !Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).EndsWith(@"Debug"))
                    {
                        StatusBox.Text = "Not in home directory, moving... ";
                        await Functions.Install.Replace();
                    }
                    else
                    {
                        StatusBox.Text = "Checking for updates...";
                        if (await Functions.Install.CheckForUpdates())
                        {
                            MessageBox.Show("It looks like there is a new version of Blaze! You can download it from Devlin.gg/Blaze");
                        }
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
                }
            }
            catch (Exception Ex)
            {
                System.Windows.MessageBox.Show(Ex.ToString());
            }
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
