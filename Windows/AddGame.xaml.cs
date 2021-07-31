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
        public static Home home;

        public Game newGame = new Game();

        public AddGame(Home home)
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate { DragMove(); };
        }

        private void SelectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Find Game Executable",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "exe",
                Filter = "EXE files|*.exe",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if ((bool) fileDialog.ShowDialog())
            {
                newGame.Filename = fileDialog.FileName;

                SelectFileBtn.Content = System.IO.Path.GetFileName(newGame.Filename);
            }
        }

        private async void AddGameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (GameTitleBox.Text != "" && AppIDBox.Text != "")
            {
                newGame.Title = GameTitleBox.Text;
                try 
                {
                    uint appID = uint.Parse(AppIDBox.Text);
                    newGame.AppID = appID;

                    Process game = new Process();
                    game.StartInfo.FileName = newGame.Filename;
                    game.Start();
                    newGame.PlainName = game.ProcessName;
                    game.Kill();


                    if (!File.Exists(Directory.GetCurrentDirectory() + "\\games.txt")) File.Create(Directory.GetCurrentDirectory() + "\\games.txt").Close();
                    TextWriter gamesTXT = new StreamWriter(Directory.GetCurrentDirectory() + "\\games.txt", true);

                    gamesTXT.WriteLine("https://devlin.gg/blaze/GI.png," + newGame.Title + "," + newGame.AppID + ",https://devlin.gg/blaze/BG.png,https://devlin.gg/blaze," + newGame.Filename + "," + newGame.PlainName);
                    gamesTXT.Close();

                    await home.UpdateGames();
                    this.Close();
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.ToString());
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

        private void GameTitleBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) { GameTitleBox.Text = ""; }

        private void AppIDBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) { AppIDBox.Text = ""; }
    }
}
