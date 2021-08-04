using DiscordRPC;
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

namespace Blaze.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private void CloseBtn_Click(object sender, RoutedEventArgs e) { this.Close(); }

        public Settings(Home home)
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate { DragMove(); };

            Functions.Discord.discord.client.ClearPresence();
            Functions.Discord.discord.client.SetPresence(new RichPresence()
            {
                Details = "Tweeking their settings...",
                Timestamps = Functions.Discord.startTime,
                Assets = new Assets()
                {
                    LargeImageKey = "nutural",
                    LargeImageText = "Devlin.gg/Blaze",
                }
            });
        }
    }
}
