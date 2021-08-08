using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Blaze.Functions
{
    class Blaze
    {
        public static void Say(Windows.Home home, string txt, string emote)
        {
            try
            {
                if (home.BlazeTxtBox.Visibility == Visibility.Hidden)
                {
                    home.BlazeCloseBtn.Visibility = Visibility.Visible;
                    home.BlazeTxtBox.Visibility = Visibility.Visible;
                    home.BlazeTxtBG.Visibility = Visibility.Visible;
                }

                ImageBrush ib = new ImageBrush();

                ib.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Assets/Blaze/Small/" + emote + "-small.png"));

                home.Blaze.Background = ib;
                home.BlazeTxtBox.Text = txt;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }

    }
}
