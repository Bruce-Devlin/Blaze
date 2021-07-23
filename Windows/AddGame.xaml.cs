using Blaze.Functions;
using Microsoft.Win32;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Blaze.Windows
{
    /// <summary>
    /// Interaction logic for AddGame.xaml
    /// </summary>
    public partial class AddGame : Window
    {
        public Home home;

        public Game newGame = new Game();

        public AddGame()
        {
            InitializeComponent();
        }

        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Find Game Executable";
            fileDialog.Filter = "EXE files|*.exe";
            fileDialog.InitialDirectory = @"C:\";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo exe = new FileInfo(fileDialog.FileName);
                newGame.Filename = exe.Name;
            }
        }

        private void AddGameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (GameTitleBox.Text != "" && AppIDBox.Text != "")
            {
                newGame.Title = GameTitleBox.Text;
                try 
                {
                    uint appID = uint.Parse(AppIDBox.Text);
                    newGame.AppID = appID;

                    SteamClient.Init(newGame.AppID);

                    Process game = new Process();
                    game.StartInfo.FileName = SteamApps.AppInstallDir(newGame.AppID) + "\\" + newGame.Filename;
                    game.Start();
                    newGame.PlainName = game.ProcessName;
                    game.Close();


                    if (!File.Exists(Directory.GetCurrentDirectory() + "\\games.txt")) File.Create(Directory.GetCurrentDirectory() + "\\games.txt");
                    TextWriter gamesTXT = new StreamWriter(Directory.GetCurrentDirectory() + "\\games.txt", true);

                    gamesTXT.WriteLine(Environment.NewLine + "https://devlin.gg/blaze/GI.png," + newGame.Title + "," + newGame.AppID + ",https://devlin.gg/blaze/GI.png," + newGame.Filename + "," + newGame.PlainName);
                    gamesTXT.Close();

                }
                catch
                {
                    //not an id
                }
            }
            else if (GameTitleBox.Text != "")
            {
                //game title empty
            }
            else
            {
                //app id empty
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
