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

namespace Blaze.Welcome
{
    /// <summary>
    /// Interaction logic for HostWindow.xaml
    /// </summary>
    public partial class HostWindow : Window
    {
        private bool waiting = false;
        public async Task BlazeSay(string Content)
        {
            for (int i = 0; i < Content.Length; i++)
            {
                BlazeTextBox.Text.Append(Content[i]);
                await Task.Delay(60);
            }
        }

        public HostWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BlazeSay("Hello, This is a test message. aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
