// Disclaimer time, this is NOT endorsed, supported or created/published by Blazing Griffin.
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
using System.Windows.Forms;



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

        public async void PreloadManager()
        {

            StatusBox.Text = "Making sure you are connected...";
            try { WebRequest testConnection = WebRequest.Create("https://devlin.gg/"); }
            catch
            {
                System.Windows.MessageBox.Show("Uh Oh... It looks like you arnt connected to the internet, check your network and then restart the app");
                Environment.Exit(0);
            }
            StatusBox.Text = "Checking installation...";
            try
            {
                if (!await Functions.Install.CheckInstallation())
                {
                    StatusBox.Text = "Installing Blaze... (this might take a second)";
                    if (!IsElevated)
                    {
                        System.Windows.MessageBox.Show("Blaze requires administrative elevation to install correctly. (After instalation Blaze does not need to be run as a Administrator.)");
                        RestartElevated();
                    }
                    else
                    {
                        Functions.Install.preload = this;
                        await Functions.Install.StartInstall();
                    }
                }
                else
                {
                    
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
